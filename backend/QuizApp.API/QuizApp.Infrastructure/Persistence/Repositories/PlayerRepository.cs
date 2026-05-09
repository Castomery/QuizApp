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
    public class PlayerRepository(AppDbContext db) : IPlayerRepository
    {
        public async Task<Player?> GetByIdAsync(Guid id) =>
        await db.Players.FindAsync(id);

        public async Task<Player> CreateAsync(Player player)
        {
            db.Players.Add(player);
            await db.SaveChangesAsync();
            return player;
        }

        public async Task UpdateAsync(Player player)
        {
            db.Players.Update(player);
            await db.SaveChangesAsync();
        }

        public async Task UpdateStatsAsync(Guid playerId, bool won)
        {
            await db.Players
                .Where(p => p.Id == playerId)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(x => x.TotalGames, x => x.TotalGames + 1)
                    .SetProperty(x => x.TotalWins, x => x.TotalWins + (won ? 1 : 0)));
        }

        public async Task<List<Player>> GetTopPlayersAsync(int count)
        {
            return await db.Players
                .OrderByDescending(p => p.TotalWins)
                .ThenByDescending(p => p.TotalGames)
                .Take(count)
                .ToListAsync();
        }

    }
}
