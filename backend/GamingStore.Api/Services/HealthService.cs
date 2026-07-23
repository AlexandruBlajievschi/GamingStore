namespace GamingStore.Api.Services;

public interface IHealthService
{
    HealthResponse GetHealth();
}

public sealed class HealthService : IHealthService
{
    public HealthResponse GetHealth()
    {
        return new HealthResponse(
            "ok",
            "Gaming Store API is running",
            DateTimeOffset.UtcNow);
    }
}

public sealed record HealthResponse(
    string Status,
    string Message,
    DateTimeOffset CheckedAt);
