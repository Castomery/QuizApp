using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
