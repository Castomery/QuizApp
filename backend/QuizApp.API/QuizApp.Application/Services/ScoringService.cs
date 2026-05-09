using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Application.Services
{
    public class ScoringService
    {
        private const int BasePoints = 1000;
        private const int MaxSpeedBonus = 500;
        private const int RoundTimeMs = 20_000;

        public int Calculate(int responseTimeMs, int streak, int difficulty)
        {
            var speedBonus = (int)(MaxSpeedBonus * (1 - (double)responseTimeMs / RoundTimeMs));
            var streakMultiplier = 1 + Math.Min(streak * 0.1, 0.5);  
            var difficultyMultiplier = 1 + difficulty * 0.1;           

            return (int)((BasePoints + speedBonus) * streakMultiplier * difficultyMultiplier);
        }
    }
}
