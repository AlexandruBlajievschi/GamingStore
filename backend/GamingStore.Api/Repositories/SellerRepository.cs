namespace GamingStore.Api.Repositories;

public interface ISellerRepository
{
    Task<IReadOnlyList<Seller>> GetAllAsync(CancellationToken cancellationToken);

    Task<Seller?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Seller?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Seller seller, CancellationToken cancellationToken);

    void Delete(Seller seller);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public sealed class SellerRepository(ApplicationDbContext dbContext) : ISellerRepository
{
    public async Task<IReadOnlyList<Seller>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Sellers
            .AsNoTracking()
            .OrderBy(seller => seller.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Seller?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Sellers
            .AsNoTracking()
            .FirstOrDefaultAsync(seller => seller.Id == id, cancellationToken);
    }

    public async Task<Seller?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Sellers
            .FirstOrDefaultAsync(seller => seller.Id == id, cancellationToken);
    }

    public async Task AddAsync(Seller seller, CancellationToken cancellationToken)
    {
        await dbContext.Sellers.AddAsync(seller, cancellationToken);
    }

    public void Delete(Seller seller)
    {
        dbContext.Sellers.Remove(seller);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
