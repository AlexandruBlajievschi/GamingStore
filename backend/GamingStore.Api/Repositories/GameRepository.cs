namespace GamingStore.Api.Repositories;

public interface IGameRepository
{
    Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken);
}

public sealed class GameRepository(ApplicationDbContext dbContext) : IGameRepository
{
    public async Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Games
            .AsNoTracking()
            .Include(game => game.Seller)
            .OrderBy(game => game.Title)
            .ToListAsync(cancellationToken);
    }
}
