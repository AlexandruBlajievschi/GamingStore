using GamingStore.Api.Models.Validation;

namespace GamingStore.Api.Models.Entities;

public sealed class User
{
    private const int NameMaxLength = 100;
    private const int EmailMaxLength = 320;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private User()
    {
    }

    private User(Guid id, string firstName, string lastName, string email, DateTime createdAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public static User Create(string firstName, string lastName, string email)
    {
        firstName = DomainText.Required(firstName, NameMaxLength);
        lastName = DomainText.Required(lastName, NameMaxLength);
        email = DomainText.Email(email, EmailMaxLength, "user");

        return new User(Guid.NewGuid(), firstName, lastName, email, DateTime.UtcNow);
    }

    public void UpdateDetails(string firstName, string lastName, string email)
    {
        firstName = DomainText.Required(firstName, NameMaxLength);
        lastName = DomainText.Required(lastName, NameMaxLength);
        email = DomainText.Email(email, EmailMaxLength, "user");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}
