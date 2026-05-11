using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Models;


namespace QuizApp.Application.Services
{
    public class AiHostService(IAiClient groqClient, IGameStateRepository gameState)
    {
        public async Task<List<Question>> GetQuestionsAsync(string topic, int difficulty, int count = 5)
        {
            var cacheKey = $"questions:{topic}:{difficulty}";
            var cached = await gameState.GetQuestionsAsync(cacheKey);

            if (cached is not null) return cached;

            var jsonTemplate = """
            [
              {
                "text": "Текст питання?",
                "options": ["Варіант A", "Варіант B", "Варіант C", "Варіант D"],
                "correctIndex": 0,
                "explanation": "Коротке пояснення чому правильно"
              }
            ]
            """;

            var prompt = $"""
                Ти — ведучий вікторини. Відповідай ВИКЛЮЧНО українською мовою.
                Згенеруй {count} питань на тему "{topic}".
                Складність: {difficulty} з 5.
                Відповідай ТІЛЬКИ валідним JSON масивом без жодного тексту до або після:
                {jsonTemplate}
                """;



            var response = await groqClient.CompleteAsync(prompt, maxTokens: 2000);

            var clean = response
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

            var questions = JsonSerializer.Deserialize<List<Question>>(clean,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new Exception("Не вдалось розпарсити питання від AI");

            await gameState.SetQuestionsAsync(cacheKey, questions);

            return questions;
        }

        public async Task<string> GetRoundCommentAsync(
            string topic, int correctCount, int totalPlayers, string fastestPlayer)
        {
            var prompt = $"""
            Ти — енергійний ведучий вікторини. Відповідай ВИКЛЮЧНО українською мовою без жодних інших мов.
            Прокоментуй результат раунду в 1-2 речення.
            Тема: {topic}
            Правильно відповіли: {correctCount} з {totalPlayers} гравців.
            Найшвидший: {fastestPlayer}.
            Будь веселим і коротким. ТІЛЬКИ українська мова.
            """;

            return await groqClient.CompleteAsync(prompt, maxTokens: 150);
        }

        public async Task<string> GetGameSummaryAsync(
            string mvp, int totalRounds, string topic)
        {
            var prompt = $"""
            Ти — ведучий вікторини. Відповідай ВИКЛЮЧНО українською мовою без жодних інших мов.
            Гра завершилась! Тема була: {topic}, зіграно раундів: {totalRounds}.
            Переможець: {mvp}.
            Напиши фінальну репліку в 2-3 речення — вітай переможця з гумором.
            ТІЛЬКИ українська мова.
            """;

            return await groqClient.CompleteAsync(prompt, maxTokens: 200);
        }
    }
}
