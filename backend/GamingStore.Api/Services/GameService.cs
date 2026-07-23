namespace GamingStore.Api.Services;

public interface IGameService
{
    Task<IReadOnlyList<GameResponse>> GetAllAsync(CancellationToken cancellationToken);
}

public sealed class GameService(IGameRepository gameRepository) : IGameService
{
    public async Task<IReadOnlyList<GameResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var games = await gameRepository.GetAllAsync(cancellationToken);

        return games
            .Select(game => new GameResponse(
                game.Id,
                game.Title,
                game.Description,
                game.Price,
                game.ReleaseDate,
                game.SellerId,
                game.Seller?.Name ?? string.Empty))
            .ToList();
    }
}

public sealed record GameResponse(
    Guid Id,
    string Title,
    string? Description,
    decimal Price,
    DateOnly? ReleaseDate,
    Guid SellerId,
    string SellerName);
