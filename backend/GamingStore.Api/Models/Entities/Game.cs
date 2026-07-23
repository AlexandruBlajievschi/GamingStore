namespace GamingStore.Api.Models.Entities;

public sealed class Game
{
    public Guid Id { get; set; }

    public Guid SellerId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public Seller? Seller { get; set; }
}
