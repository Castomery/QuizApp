using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Interfaces
{
    public interface IGameSessionRepository
    {
        Task<GameSession> CreateAsync(GameSession session);
        Task<GameSession?> GetByCodeAsync(string code);
        Task AddPlayerSessionAsync(PlayerSession playerSession);

        Task<GameSession?> GetByIdAsync(Guid id);
        Task UpdateStatusAsync(Guid sessionId, GameStatus status, DateTime finishedAt);
        Task UpdatePlayerSessionResultAsync(Guid playerSessionId, int finalScore, int finalRank, int correctAnswers);
    }
}
