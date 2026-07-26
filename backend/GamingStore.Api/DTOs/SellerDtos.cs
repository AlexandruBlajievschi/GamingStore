namespace GamingStore.Api.DTOs;

public sealed record CreateSellerRequest(string Name, string Email, string? Description);

public sealed record UpdateSellerRequest(string Name, string Email, string? Description);

public sealed record SellerResponse(
    Guid Id,
    string Name,
    string Email,
    string? Description,
    DateTime CreatedAt);
