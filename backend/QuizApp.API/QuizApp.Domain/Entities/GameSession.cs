using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Domain.Entities
{
    public class GameSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RoomCode { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int Difficulty { get; set; } = 1;
        public GameStatus Status { get; set; } = GameStatus.Waiting;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }

        public ICollection<PlayerSession> PlayerSessions { get; set; } = [];
    }

    public enum GameStatus { Waiting, InProgress, Finished}
}
