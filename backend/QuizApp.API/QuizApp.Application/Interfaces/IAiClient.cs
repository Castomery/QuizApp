using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Application.Interfaces
{
    public interface IAiClient
    {
        Task<string> CompleteAsync(string prompt, int maxTokens = 1000);
    }
}
