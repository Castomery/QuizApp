using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Models;

namespace QuizApp.Application.Interfaces
{
    public interface IGameStateRepository
    {
        Task<GameRoomState?> GetAsync(string roomCode);
        Task SetAsync(GameRoomState state);
        Task DeleteAsync(string roomCode);

        Task SetConnectionRoomAsync(string connectionId, string roomCode);
        Task<string?> GetRoomByConnectionAsync(string connectionId);
        Task DeleteConnectionAsync(string connectionId);

        Task<List<Question>?> GetQuestionsAsync(string cacheKey);
        Task SetQuestionsAsync(string cacheKey, List<Question> questions);
    }
}
