namespace GamingStore.Api.Models.Entities;

public sealed class Seller
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<Game> Games { get; set; } = [];
}
