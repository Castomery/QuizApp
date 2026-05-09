using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Interfaces
{
    public interface IGameService
    {
        Task<GameSession> CreateRoomAsync(string topic, int difficulty);
        Task<GameSession?> GetRoomByCodeAsync(string code);
        Task<PlayerSession> JoinRoomAsync(string roomCodem, string playerName);

        Task SaveGameResultsAsync(string roomCode, List<ConnectedPlayer> players);
    }
}
