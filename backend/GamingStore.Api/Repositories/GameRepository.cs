namespace GamingStore.Api.Repositories;

public interface IGameRepository
{
    Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken);

    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Game?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> SellerExistsAsync(Guid sellerId, CancellationToken cancellationToken);

    Task AddAsync(Game game, CancellationToken cancellationToken);

    void Delete(Game game);

    Task SaveChangesAsync(CancellationToken cancellationToken);
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

    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Games
            .AsNoTracking()
            .Include(game => game.Seller)
            .FirstOrDefaultAsync(game => game.Id == id, cancellationToken);
    }

    public async Task<Game?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Games
            .Include(game => game.Seller)
            .FirstOrDefaultAsync(game => game.Id == id, cancellationToken);
    }

    public async Task<bool> SellerExistsAsync(Guid sellerId, CancellationToken cancellationToken)
    {
        return await dbContext.Sellers
            .AnyAsync(seller => seller.Id == sellerId, cancellationToken);
    }

    public async Task AddAsync(Game game, CancellationToken cancellationToken)
    {
        await dbContext.Games.AddAsync(game, cancellationToken);
    }

    public void Delete(Game game)
    {
        dbContext.Games.Remove(game);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
