using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Repositories
{
    public class GameSessionRepository(AppDbContext db) : IGameSessionRepository
    {
        public async Task<GameSession> CreateAsync(GameSession session)
        {
            db.GameSessions.Add(session);
            await db.SaveChangesAsync();
            return session;
        }

        public async Task<GameSession?> GetByCodeAsync(string code) =>
            await db.GameSessions
                .Include(s => s.PlayerSessions)
                .ThenInclude(ps => ps.Player)
                .FirstOrDefaultAsync(s => s.RoomCode == code);

        public async Task AddPlayerSessionAsync(PlayerSession playerSession)
        {
            db.PlayerSessions.Add(playerSession);
            await db.SaveChangesAsync();
        }
    }
}
