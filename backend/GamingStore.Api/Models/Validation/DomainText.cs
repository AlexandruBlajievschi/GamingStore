using System.Net.Mail;

namespace GamingStore.Api.Models.Validation;

internal static class DomainText
{
    public static string Required(string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("A required text value cannot be empty.");
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new DomainValidationException(
                $"A text value cannot be longer than {maxLength} characters.");
        }

        return value;
    }

    public static string? Optional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new DomainValidationException(
                $"A text value cannot be longer than {maxLength} characters.");
        }

        return value;
    }

    public static string Email(string value, int maxLength, string ownerName)
    {
        value = Required(value, maxLength).ToLowerInvariant();
        var invalidEmailMessage = $"A {ownerName} email must be a valid email address.";

        try
        {
            var address = new MailAddress(value);

            if (!string.Equals(address.Address, value, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainValidationException(invalidEmailMessage);
            }
        }
        catch (FormatException)
        {
            throw new DomainValidationException(invalidEmailMessage);
        }

        return value;
    }
}
