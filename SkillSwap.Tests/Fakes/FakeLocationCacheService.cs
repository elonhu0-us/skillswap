using SkillSwap.Api.Services.Location;

public class FakeLocationCacheService : ILocationCacheService
{
    private readonly Dictionary<int, (double lat, double lng)> _store = new();
    public Task SetLocationAsync(int userId, double lat, double lng)
    {
        _store[userId] = (lat, lng);
        return Task.CompletedTask;
    }
    public Task<(double lat, double lng)?> GetLocationAsync(int userId)
    {
        if (_store.TryGetValue(userId, out var v)) return Task.FromResult<(double, double)?>((v.lat, v.lng));
        return Task.FromResult<(double, double)?> (null);
    }
}
