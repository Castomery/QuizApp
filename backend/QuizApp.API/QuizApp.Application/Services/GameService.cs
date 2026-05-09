using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Services
{
    public class GameService(IGameSessionRepository sessionRepo, IPlayerRepository playerRepo) : IGameService
    {
        public async Task<GameSession> CreateRoomAsync(string topic, int difficulty)
        {
            var session = new GameSession
            {
                RoomCode = GenerateRoomCode(),
                Topic = topic,
                Difficulty = Math.Clamp(difficulty, 1, 5)
            };

            return await sessionRepo.CreateAsync(session);
        }

        public async Task<GameSession?> GetRoomByCodeAsync(string code) =>
            await sessionRepo.GetByCodeAsync(code.ToUpper());

        public async Task<PlayerSession> JoinRoomAsync(string roomCode, string playerName)
        {
            var session = await sessionRepo.GetByCodeAsync(roomCode.ToUpper())
                ?? throw new InvalidOperationException($"Кімната {roomCode} не знайдена");

            if (session.Status != GameStatus.Waiting)
                throw new InvalidOperationException("Гра вже розпочалась");

            var player = await playerRepo.CreateAsync(new Player
            {
                Username = playerName,
                AvatarColor = GetRandomColor()
            });

            var playerSession = new PlayerSession
            {
                PlayerId = player.Id,
                GameSessionId = session.Id,
                Player = player
            };

            await sessionRepo.AddPlayerSessionAsync(playerSession);

            return playerSession;
        }

        public async Task SaveGameResultsAsync(string roomCode, List<ConnectedPlayer> players)
        {
            var session = await sessionRepo.GetByCodeAsync(roomCode.ToUpper())
                ?? throw new InvalidOperationException($"Сесія {roomCode} не знайдена");

            await sessionRepo.UpdateStatusAsync(
                session.Id,
                GameStatus.Finished,
                DateTime.UtcNow);

            var ranked = players
                .OrderByDescending(p => p.Score)
                .Select((p, i) => (Player: p, Rank: i + 1))
                .ToList();

            foreach (var (player, rank) in ranked)
            {
                var playerSession = session.PlayerSessions
                    .FirstOrDefault(ps => ps.PlayerId == player.PlayerId);

                if (playerSession is null) continue;

                var correctAnswers = player.Score > 0
                    ? (int)Math.Round(player.Score / 1000.0)
                    : 0;

                await sessionRepo.UpdatePlayerSessionResultAsync(
                    playerSession.Id,
                    player.Score,
                    rank,
                    player.CorrectAnswers);

                await playerRepo.UpdateStatsAsync(
                    player.PlayerId,
                    won: rank == 1);
            }
        }

        private static string GenerateRoomCode() =>
            Random.Shared.Next(100000, 999999).ToString();

        private static string GetRandomColor()
        {
            string[] colors = ["#7F77DD", "#E24B4A", "#1a9e6e", "#f5a623", "#2f6ec8"];
            return colors[Random.Shared.Next(colors.Length)];
        }
    }
}
