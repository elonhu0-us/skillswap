namespace SkillSwap.Api.Services.Location
{
    public interface ILocationCacheService
    {
        Task SetLocationAsync(int userId, double lat, double lng);
        Task<(double lat, double lng)?> GetLocationAsync(int userId);
    }
}