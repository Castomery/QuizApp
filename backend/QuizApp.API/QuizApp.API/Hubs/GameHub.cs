using Microsoft.AspNetCore.SignalR;
using QuizApp.Application.Interfaces;
using QuizApp.Application.Services;
using QuizApp.Domain.Entities;

namespace QuizApp.API.Hubs
{
    public class GameHub(
    IGameStateRepository gameState,
    IGameSessionRepository sessionRepo,
    ScoringService scoringService,
    AiHostService aiHost,
    IGameService gameService) : Hub
    {

        public async Task JoinRoom(string roomCode, string token)
        {

            Guid playerSessionId;
            try
            {
                playerSessionId = new Guid(Convert.FromBase64String(token));
            }
            catch
            {
                throw new HubException("Невалідний токен");
            }

            var session = await sessionRepo.GetByCodeAsync(roomCode.ToUpper())
                ?? throw new HubException($"Кімната {roomCode} не існує");

            var playerSession = session.PlayerSessions
                .FirstOrDefault(ps => ps.Id == playerSessionId)
                ?? throw new HubException("Токен не належить цій кімнаті");

            var player = playerSession.Player;

            var state = await gameState.GetAsync(roomCode) ?? new GameRoomState
            {
                RoomCode = roomCode,
                Topic = session.Topic,
                Difficulty = session.Difficulty
            };

            if (state.Phase != RoomPhase.Waiting)
                throw new HubException("Гра вже розпочалась, приєднатися неможливо");

            if (!state.Players.Any(p => p.ConnectionId == Context.ConnectionId))
            {
                state.Players.Add(new ConnectedPlayer
                {
                    ConnectionId = Context.ConnectionId,
                    PlayerId = player.Id,
                    Username = player.Username,
                    AvatarColor = player.AvatarColor
                });
            }

            await gameState.SetAsync(state);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            await gameState.SetConnectionRoomAsync(Context.ConnectionId, roomCode.ToUpper());

            await Clients.Group(roomCode).SendAsync("PlayerJoined", new
            {
                player.Username,
                player.AvatarColor,
                totalPlayers = state.Players.Count
            });

            await Clients.Caller.SendAsync("RoomState", new
            {
                state.RoomCode,
                state.Topic,
                state.Difficulty,
                players = state.Players.Select(p => new
                {
                    p.Username,
                    p.AvatarColor,
                    p.Score
                })
            });
        }

        public async Task StartGame(string roomCode)
        {
            var state = await gameState.GetAsync(roomCode)
                ?? throw new HubException("Кімната не знайдена");

            if (state.Players.Count < 1)
                throw new HubException("Потрібен хоча б один гравець");

            if (state.Phase != RoomPhase.Waiting)
                throw new HubException("Гра вже йде");

            await Clients.Group(roomCode).SendAsync("GeneratingQuestions", new
            {
                message = "AI готує питання..."
            });

            var questions = await aiHost.GetQuestionsAsync(
                state.Topic, state.Difficulty, state.TotalRounds);

            state.Phase = RoomPhase.InProgress;
            state.CurrentRound = 1;
            state.CurrentQuestion = questions[0];
            await gameState.SetAsync(state);

            await gameState.SetQuestionsAsync(
            $"{roomCode}:questions",
            questions);

            await Clients.Group(roomCode).SendAsync("GameStarted", new
            {
                state.Topic,
                state.Difficulty,
                state.TotalRounds
            });

            await Clients.Group(roomCode).SendAsync("QuestionStarted", new
            {
                round = state.CurrentRound,
                total = state.TotalRounds,
                text = state.CurrentQuestion.Text,
                options = state.CurrentQuestion.Options,
                timeoutSec = 20
            });
        }

        public async Task SubmitAnswer(string roomCode, int answerIndex, int responseTimeMs)
        {
            var state = await gameState.GetAsync(roomCode)
                ?? throw new HubException("Кімната не знайдена");

            var player = state.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId)
                ?? throw new HubException("Гравець не знайдений");

            if (player.HasAnswered)
                throw new HubException("Ти вже відповів на це питання");

            player.HasAnswered = true;
            player.LastAnswerCorrect = answerIndex == state.CurrentQuestion.CorrectIndex;

            var isCorrect = player.LastAnswerCorrect;
            var points = isCorrect
                ? scoringService.Calculate(responseTimeMs, player.Streak, state.Difficulty)
                : 0;


            if (isCorrect)
            {
                player.Score += points;
                player.Streak++;
                player.CorrectAnswers++;
                if (player.FastestTimeMs == 0 || responseTimeMs < player.FastestTimeMs)
                    player.FastestTimeMs = responseTimeMs;
            }
            else
            {
                player.Streak = 0;
            }

            await gameState.SetAsync(state);

            await Clients.Caller.SendAsync("AnswerResult", new
            {
                isCorrect,
                pointsEarned = points,
                newScore = player.Score,
                streak = player.Streak
            });

            var leaderboard = state.Players
                .OrderByDescending(p => p.Score)
                .Select((p, i) => new
                {
                    rank = i + 1,
                    p.Username,
                    p.AvatarColor,
                    p.Score,
                    p.HasAnswered
                });

            await Clients.Group(roomCode).SendAsync("LeaderboardUpdated", leaderboard);

            if (state.Players.All(p => p.HasAnswered))
                await EndRound(roomCode, state);
        }

        private async Task EndRound(string roomCode, GameRoomState state)
        {

            var fastest = state.Players
            .Where(p => p.LastAnswerCorrect)
            .MinBy(p => p.FastestTimeMs);

            var correctCount = state.Players.Count(p => p.LastAnswerCorrect);
            var comment = await aiHost.GetRoundCommentAsync(
                state.Topic,
                correctCount,
                state.Players.Count,
                fastest?.Username ?? "ніхто");

            await Clients.Group(roomCode).SendAsync("RoundEnded", new
            {
                round = state.CurrentRound,
                correctAnswerIndex = state.CurrentQuestion!.CorrectIndex,
                explanation = state.CurrentQuestion.Explanation,
                aiComment = comment
            });

            if (state.CurrentRound >= state.TotalRounds)
            {
                state.Phase = RoomPhase.Finished;
                await gameState.SetAsync(state);

                var mvp = state.Players.MaxBy(p => p.Score);

                var summary = await aiHost.GetGameSummaryAsync(
                    mvp?.Username ?? "невідомий",
                    state.TotalRounds,
                    state.Topic);

                var finalLeaderboard = state.Players
                    .OrderByDescending(p => p.Score)
                    .Select((p, i) => new
                    {
                        rank = i + 1,
                        p.Username,
                        p.Score
                    });

                await Clients.Group(roomCode).SendAsync("GameFinished", new
                {
                    leaderboard = finalLeaderboard,
                    mvp = mvp?.Username,
                    aiSummary = summary
                });

                await gameService.SaveGameResultsAsync(roomCode, state.Players);

                await gameState.DeleteAsync(roomCode);
            }
            else
            {

                var questions = await gameState.GetQuestionsAsync($"{roomCode}:questions")
                ?? throw new HubException("Питання не знайдені");


                state.CurrentRound++;
                state.CurrentQuestion = questions[state.CurrentRound - 1];
                state.Players.ForEach(p =>
                {
                    p.HasAnswered = false;
                    p.LastAnswerCorrect = false;
                    p.FastestTimeMs = 0;
                });

                await gameState.SetAsync(state);

                await Clients.Group(roomCode).SendAsync("QuestionStarted", new
                {
                    round = state.CurrentRound,
                    total = state.TotalRounds,
                    text = state.CurrentQuestion.Text,
                    options = state.CurrentQuestion.Options,
                    timeoutSec = 20
                });
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var roomCode = await gameState.GetRoomByConnectionAsync(Context.ConnectionId);

            if (roomCode is not null)
            {
                var state = await gameState.GetAsync(roomCode);

                if (state is not null)
                {
                    var player = state.Players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);

                    if (player is not null)
                    {
                        state.Players.Remove(player);
                        await gameState.SetAsync(state);

                        await Clients.Group(roomCode).SendAsync("PlayerLeft", new
                        {
                            player.Username,
                            remainingPlayers = state.Players.Count
                        });
                    }
                }

                await gameState.DeleteConnectionAsync(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
