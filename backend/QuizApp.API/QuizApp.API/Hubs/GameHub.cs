using Microsoft.AspNetCore.SignalR;
using QuizApp.Application.Interfaces;
using QuizApp.Application.Services;
using QuizApp.Domain.Entities;

namespace QuizApp.API.Hubs
{
    public class GameHub(
    IGameStateRepository gameState,
    IGameSessionRepository sessionRepo,
    ScoringService scoringService) : Hub
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

            state.Phase = RoomPhase.InProgress;
            state.CurrentRound = 1;
            await gameState.SetAsync(state);

            await Clients.Group(roomCode).SendAsync("GameStarted", new
            {
                state.Topic,
                state.Difficulty,
                state.TotalRounds
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

            // TODO Етап 3: правильний індекс буде з AI питання
            var isCorrect = answerIndex == 0;
            var points = isCorrect
                ? scoringService.Calculate(responseTimeMs, player.Streak, state.Difficulty)
                : 0;

            if (isCorrect)
            {
                player.Score += points;
                player.Streak++;
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
            await Clients.Group(roomCode).SendAsync("RoundEnded", new
            {
                round = state.CurrentRound,
                correctAnswerIndex = 0 // TODO Етап 3: з AI питання
            });

            if (state.CurrentRound >= state.TotalRounds)
            {
                state.Phase = RoomPhase.Finished;
                await gameState.SetAsync(state);

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
                    mvp = state.Players.MaxBy(p => p.Score)?.Username
                });
            }
            else
            {
                state.CurrentRound++;
                state.Players.ForEach(p => p.HasAnswered = false);
                await gameState.SetAsync(state);
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
