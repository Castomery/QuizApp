using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Models;
using StackExchange.Redis;

namespace QuizApp.Infrastructure.Cache
{
    public class RedisGameStateRepository(IConnectionMultiplexer redis) : IGameStateRepository
    {
        private readonly IDatabase _db = redis.GetDatabase();
        private const string Prefix = "room:";
        private const string ConnPrefix = "conn:";
        private readonly TimeSpan _ttl = TimeSpan.FromHours(2);
        private readonly TimeSpan _connTtl = TimeSpan.FromHours(2);

        public async Task<GameRoomState?> GetAsync(string roomCode)
        {
            var value = await _db.StringGetAsync(Prefix + roomCode);
            if (value.IsNullOrEmpty) return null;
            return JsonSerializer.Deserialize<GameRoomState>(value!);
        }

        public async Task SetAsync(GameRoomState state) =>
            await _db.StringSetAsync(
                Prefix + state.RoomCode,
                JsonSerializer.Serialize(state),
                _ttl);

        public async Task DeleteAsync(string roomCode) =>
            await _db.KeyDeleteAsync(Prefix + roomCode);

        public async Task SetConnectionRoomAsync(string connectionId, string roomCode) =>
        await _db.StringSetAsync(ConnPrefix + connectionId, roomCode, _connTtl);

        public async Task<string?> GetRoomByConnectionAsync(string connectionId)
        {
            var value = await _db.StringGetAsync(ConnPrefix + connectionId);
            return value.IsNullOrEmpty ? null : value.ToString();
        }

        public async Task DeleteConnectionAsync(string connectionId) =>
            await _db.KeyDeleteAsync(ConnPrefix + connectionId);

        public async Task<List<Question>?> GetQuestionsAsync(string cacheKey)
        {
            var value = await _db.StringGetAsync("questions:" + cacheKey);
            if (value.IsNullOrEmpty) return null;
            return JsonSerializer.Deserialize<List<Question>>(value!);
        }

        public async Task SetQuestionsAsync(string cacheKey, List<Question> questions) =>
            await _db.StringSetAsync(
                "questions:" + cacheKey,
                JsonSerializer.Serialize(questions),
                TimeSpan.FromHours(1));
    }
}
