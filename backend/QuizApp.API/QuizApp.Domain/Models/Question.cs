using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Domain.Models
{
    public class Question
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = [];
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}
