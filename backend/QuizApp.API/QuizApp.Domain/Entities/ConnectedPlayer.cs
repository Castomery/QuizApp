using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Domain.Entities
{
    public class ConnectedPlayer
    {
        public string ConnectionId { get; set; } = string.Empty;
        public Guid PlayerId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AvatarColor { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
        public int Streak { get; set; } = 0;
        public bool HasAnswered { get; set; } = false;
    }
}
