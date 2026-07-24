namespace GamingStore.Api.Services;

public interface IGameService
{
    Task<IReadOnlyList<GameResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<GameResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<GameResponse> CreateAsync(CreateGameRequest request, CancellationToken cancellationToken);

    Task<GameResponse> UpdateAsync(Guid id, UpdateGameRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class GameService(IGameRepository gameRepository) : IGameService
{
    public async Task<IReadOnlyList<GameResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var games = await gameRepository.GetAllAsync(cancellationToken);

        return games
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<GameResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await GetExistingGameAsync(id, trackChanges: false, cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameResponse> CreateAsync(CreateGameRequest request, CancellationToken cancellationToken)
    {
        await EnsureSellerExistsAsync(request.SellerId, cancellationToken);

        var game = Game.Create(
            request.SellerId,
            request.Title,
            request.Description,
            request.Price,
            request.ReleaseDate);

        await gameRepository.AddAsync(game, cancellationToken);
        await gameRepository.SaveChangesAsync(cancellationToken);

        var createdGame = await GetExistingGameAsync(game.Id, trackChanges: false, cancellationToken);

        return MapToResponse(createdGame);
    }

    public async Task<GameResponse> UpdateAsync(
        Guid id,
        UpdateGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = await GetExistingGameAsync(id, trackChanges: true, cancellationToken);

        game.UpdateDetails(
            request.Title,
            request.Description,
            request.Price,
            request.ReleaseDate);

        await gameRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var game = await GetExistingGameAsync(id, trackChanges: true, cancellationToken);

        gameRepository.Delete(game);
        await gameRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Game> GetExistingGameAsync(
        Guid id,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        var game = trackChanges
            ? await gameRepository.GetTrackedByIdAsync(id, cancellationToken)
            : await gameRepository.GetByIdAsync(id, cancellationToken);

        return game ?? throw new ResourceNotFoundException($"Game '{id}' was not found.");
    }

    private async Task EnsureSellerExistsAsync(Guid sellerId, CancellationToken cancellationToken)
    {
        var exists = await gameRepository.SellerExistsAsync(sellerId, cancellationToken);

        if (!exists)
        {
            throw new ResourceNotFoundException($"Seller '{sellerId}' was not found.");
        }
    }

    private static GameResponse MapToResponse(Game game)
    {
        return new GameResponse(
            game.Id,
            game.Title,
            game.Description,
            game.Price,
            game.ReleaseDate,
            game.SellerId,
            game.Seller?.Name ?? string.Empty);
    }
}

public sealed record CreateGameRequest(
    Guid SellerId,
    string Title,
    string? Description,
    decimal Price,
    DateOnly? ReleaseDate);

public sealed record UpdateGameRequest(
    string Title,
    string? Description,
    decimal Price,
    DateOnly? ReleaseDate);

public sealed record GameResponse(
    Guid Id,
    string Title,
    string? Description,
    decimal Price,
    DateOnly? ReleaseDate,
    Guid SellerId,
    string SellerName);
