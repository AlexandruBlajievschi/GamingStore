namespace GamingStore.Api.Services;

public interface ISellerService
{
    Task<IReadOnlyList<SellerResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<SellerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<SellerResponse> CreateAsync(CreateSellerRequest request, CancellationToken cancellationToken);

    Task<SellerResponse> UpdateAsync(Guid id, UpdateSellerRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class SellerService(ISellerRepository sellerRepository) : ISellerService
{
    public async Task<IReadOnlyList<SellerResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var sellers = await sellerRepository.GetAllAsync(cancellationToken);

        return sellers
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<SellerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var seller = await GetExistingSellerAsync(id, trackChanges: false, cancellationToken);

        return MapToResponse(seller);
    }

    public async Task<SellerResponse> CreateAsync(CreateSellerRequest request, CancellationToken cancellationToken)
    {
        var seller = Seller.Create(request.Name, request.Email, request.Description);

        await sellerRepository.AddAsync(seller, cancellationToken);
        await sellerRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(seller);
    }

    public async Task<SellerResponse> UpdateAsync(
        Guid id,
        UpdateSellerRequest request,
        CancellationToken cancellationToken)
    {
        var seller = await GetExistingSellerAsync(id, trackChanges: true, cancellationToken);

        seller.UpdateDetails(request.Name, request.Email, request.Description);
        await sellerRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(seller);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var seller = await GetExistingSellerAsync(id, trackChanges: true, cancellationToken);

        sellerRepository.Delete(seller);
        await sellerRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Seller> GetExistingSellerAsync(
        Guid id,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        var seller = trackChanges
            ? await sellerRepository.GetTrackedByIdAsync(id, cancellationToken)
            : await sellerRepository.GetByIdAsync(id, cancellationToken);

        return seller ?? throw new ResourceNotFoundException($"Seller '{id}' was not found.");
    }

    private static SellerResponse MapToResponse(Seller seller)
    {
        return new SellerResponse(
            seller.Id,
            seller.Name,
            seller.Email,
            seller.Description,
            seller.CreatedAt);
    }
}

public sealed record CreateSellerRequest(string Name, string Email, string? Description);

public sealed record UpdateSellerRequest(string Name, string Email, string? Description);

public sealed record SellerResponse(
    Guid Id,
    string Name,
    string Email,
    string? Description,
    DateTime CreatedAt);
