namespace GamingStore.Api.Models.Entities;

public sealed class User
{
    private const int NameMaxLength = 100;
    private const int EmailMaxLength = 320;

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
        firstName = NormalizeRequiredText(firstName, nameof(firstName), NameMaxLength);
        lastName = NormalizeRequiredText(lastName, nameof(lastName), NameMaxLength);
        email = NormalizeEmail(email, nameof(email));

        return new User(Guid.NewGuid(), firstName, lastName, email, DateTime.UtcNow);
    }

    private static string NormalizeRequiredText(string value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A required text value cannot be empty.", parameterName);
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"A text value cannot be longer than {maxLength} characters.", parameterName);
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
                throw new ArgumentException("A user email must be a valid email address.", parameterName);
            }
        }
        catch (FormatException exception)
        {
            throw new ArgumentException("A user email must be a valid email address.", parameterName, exception);
        }

        return value;
    }
}
