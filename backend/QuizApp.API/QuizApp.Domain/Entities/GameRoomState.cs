using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizApp.Domain.Models;

namespace QuizApp.Domain.Entities
{
    public class GameRoomState
    {
        public string RoomCode { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int Difficulty { get; set; }
        public List<ConnectedPlayer> Players { get; set; } = [];
        public int CurrentRound { get; set; } = 0;
        public int TotalRounds { get; set; } = 5;
        public RoomPhase Phase { get; set; } = RoomPhase.Waiting;

        public Question? CurrentQuestion { get; set; }
    }
}
