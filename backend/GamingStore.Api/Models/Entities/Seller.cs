using GamingStore.Api.Models.Validation;

namespace GamingStore.Api.Models.Entities;

public sealed class Seller
{
    private const int NameMaxLength = 150;
    private const int EmailMaxLength = 320;
    private const int DescriptionMaxLength = 1000;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private Seller()
    {
    }

    private Seller(Guid id, string name, string email, string? description, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Description = description;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public ICollection<Game> Games { get; private set; } = [];

    public static Seller Create(string name, string email, string? description = null)
    {
        name = DomainText.Required(name, NameMaxLength);
        email = DomainText.Email(email, EmailMaxLength, "seller");
        description = DomainText.Optional(description, DescriptionMaxLength);

        return new Seller(Guid.NewGuid(), name, email, description, DateTime.UtcNow);
    }

    public void UpdateDetails(string name, string email, string? description = null)
    {
        name = DomainText.Required(name, NameMaxLength);
        email = DomainText.Email(email, EmailMaxLength, "seller");
        description = DomainText.Optional(description, DescriptionMaxLength);

        Name = name;
        Email = email;
        Description = description;
    }
}
