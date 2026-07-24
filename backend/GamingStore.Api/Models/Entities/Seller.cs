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
        name = NormalizeRequiredText(name, nameof(name), NameMaxLength);
        email = NormalizeEmail(email, nameof(email));
        description = NormalizeOptionalText(description, nameof(description), DescriptionMaxLength);

        return new Seller(Guid.NewGuid(), name, email, description, DateTime.UtcNow);
    }

    public void UpdateDetails(string name, string email, string? description = null)
    {
        name = NormalizeRequiredText(name, nameof(name), NameMaxLength);
        email = NormalizeEmail(email, nameof(email));
        description = NormalizeOptionalText(description, nameof(description), DescriptionMaxLength);

        Name = name;
        Email = email;
        Description = description;
    }

    private static string NormalizeRequiredText(string value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("A required text value cannot be empty.");
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new DomainValidationException($"A text value cannot be longer than {maxLength} characters.");
        }

        return value;
    }

    private static string NormalizeEmail(string value, string parameterName)
    {
        value = NormalizeRequiredText(value, parameterName, EmailMaxLength).ToLowerInvariant();

        try
        {
            var address = new System.Net.Mail.MailAddress(value);

            if (!string.Equals(address.Address, value, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainValidationException("A seller email must be a valid email address.");
            }
        }
        catch (FormatException)
        {
            throw new DomainValidationException("A seller email must be a valid email address.");
        }

        return value;
    }

    private static string? NormalizeOptionalText(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new DomainValidationException($"A text value cannot be longer than {maxLength} characters.");
        }

        return value;
    }
}
