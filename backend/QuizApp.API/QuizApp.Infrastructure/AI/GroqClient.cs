using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using QuizApp.Application.Interfaces;

namespace QuizApp.Infrastructure.AI
{
    public class GroqClient : IAiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public GroqClient(HttpClient http, IConfiguration config)
        {
            _httpClient = http;
            _model = config["Groq:Model"] ?? "llama-3.3-70b-versatile";
            _httpClient.BaseAddress = new Uri("https://api.groq.com");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["Groq:ApiKey"]}");
        }

        public async Task<string> CompleteAsync(string prompt, int maxTokens = 1000)
        {
            var request = new
            {
                model = _model,
                max_tokens = maxTokens,
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = """
                        Ти — україномовний ведучий розважальної вікторини на телебаченні.
                        ОБОВ'ЯЗКОВІ ПРАВИЛА — порушення неприпустиме:
                        1. Відповідай ВИКЛЮЧНО українською мовою.
                        2. Забороняється використовувати будь-які англійські, китайські або інші іноземні слова.
                        3. Якщо не знаєш слова — заміни синонімом або опиши по-іншому.
                        4. Стиль: енергійний, веселий, як на живому телешоу.
                        5. Ніколи не починай речення з іноземного слова.
                        """
                    },
                    new { role = "user", content = prompt }
                }       
            };

            var response = await _httpClient.PostAsJsonAsync(
                "/openai/v1/chat/completions", request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GroqResponse>(json);
            return result?.Choices?[0]?.Message?.Content ?? throw new Exception("Порожня відповідь від Groq");
        }
    }
    file class GroqResponse
    {
        [JsonPropertyName("choices")]
        public List<GroqChoice>? Choices { get; set; }
    }

    file class GroqChoice
    {
        [JsonPropertyName("message")]
        public GroqMessage? Message { get; set; }
    }

    file class GroqMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
