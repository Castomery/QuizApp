using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Domain.Entities
{
    public class Player
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string AvatarColor { get; set; } = "#7F77DD";
        public int TotalGames { get; set; }
        public int TotalWins { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // навігаційна властивість
        public ICollection<PlayerSession> PlayerSessions { get; set; } = [];
    }
}
