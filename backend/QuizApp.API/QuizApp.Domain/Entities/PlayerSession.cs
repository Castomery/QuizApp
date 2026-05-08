using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Domain.Entities
{
    public class PlayerSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PlayerId { get; set; }
        public Guid GameSessionId { get; set; }
        public int FinalScore { get; set; }
        public int FinalRank { get; set; }
        public int CorrectAnswers { get; set; }

        public Player Player { get; set; } = null!;
        public GameSession GameSession { get; set; } = null!;
        public ICollection<Answer> Answers { get; set; } = [];
    }
}
