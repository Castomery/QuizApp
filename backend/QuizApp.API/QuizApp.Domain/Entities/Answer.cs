using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Domain.Entities
{
    public class Answer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PlayerSessionId { get; set; }
        public int QuestionIndex { get; set; }
        public int AnswerIndex { get; set; }
        public bool IsCorrect { get; set; }
        public int ResponseTimeMs { get; set; }
        public int PointsEarned { get; set; }

        public PlayerSession PlayerSession { get; set; } = null!;
    }
}
