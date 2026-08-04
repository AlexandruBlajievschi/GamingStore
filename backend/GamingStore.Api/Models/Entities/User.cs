using GamingStore.Api.Models.Validation;
using Microsoft.AspNetCore.Identity;

namespace GamingStore.Api.Models.Entities;

public sealed class User : IdentityUser<Guid>
{
    private const int NameMaxLength = 100;
    private const int EmailMaxLength = 320;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private User()
    {
    }

    private User(Guid id, string firstName, string lastName, string email, DateTime createdAt)
        : base(email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedAt = createdAt;
    }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

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
        UserName = email;
    }
}
