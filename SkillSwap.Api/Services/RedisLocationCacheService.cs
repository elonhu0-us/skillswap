using StackExchange.Redis;
using System.Text.Json;

namespace SkillSwap.Api.Services.Location
{
    public class RedisLocationCacheService : ILocationCacheService
    {
        private readonly IDatabase _redis;

        public RedisLocationCacheService(IConnectionMultiplexer mux)
        {
            _redis = mux.GetDatabase();
        }

        public async Task SetLocationAsync(int userId, double lat, double lng)
        {
            var obj = new { lat, lng };
            await _redis.StringSetAsync($"user:loc:{userId}", JsonSerializer.Serialize(obj));
        }

        public async Task<(double lat, double lng)?> GetLocationAsync(int userId)
        {
            var value = await _redis.StringGetAsync($"user:loc:{userId}");
            if (value.IsNullOrEmpty) return null;

            var obj = JsonSerializer.Deserialize<LocationObj>((string)value!);
            return (obj!.lat, obj.lng);
        }

        private class LocationObj
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
    }
}