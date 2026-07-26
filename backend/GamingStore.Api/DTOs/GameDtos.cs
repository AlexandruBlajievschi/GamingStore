namespace GamingStore.Api.DTOs;

public sealed record CreateGameRequest(
    Guid SellerId,
    string Title,
    string? Description,
    decimal Price,
    DateOnly? ReleaseDate,
    string? CoverImageUrl = null);

public sealed record UpdateGameRequest(
    string Title,
    string? Description,
    decimal Price,
    DateOnly? ReleaseDate,
    string? CoverImageUrl = null);

public sealed record GameResponse(
    Guid Id,
    string Slug,
    string Title,
    string? Description,
    decimal Price,
    DateOnly? ReleaseDate,
    string? CoverImageUrl,
    Guid SellerId,
    string SellerName);
