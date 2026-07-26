namespace GamingStore.Api.DTOs;

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
