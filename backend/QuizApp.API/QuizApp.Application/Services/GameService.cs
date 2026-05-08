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

        private static string GenerateRoomCode() =>
            Random.Shared.Next(100000, 999999).ToString();

        private static string GetRandomColor()
        {
            string[] colors = ["#7F77DD", "#E24B4A", "#1a9e6e", "#f5a623", "#2f6ec8"];
            return colors[Random.Shared.Next(colors.Length)];
        }
    }
}
