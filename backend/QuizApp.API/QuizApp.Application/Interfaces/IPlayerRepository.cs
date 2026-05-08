using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Interfaces
{
    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(Guid id);
        Task<Player> CreateAsync(Player player);
        Task UpdateAsync(Player player);
    }
}
