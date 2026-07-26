using System.Globalization;
using System.Text;
using GamingStore.Api.Models.Validation;

namespace GamingStore.Api.Models.Entities;

public sealed class Game
{
    private const int TitleMaxLength = 200;
    private const int SlugMaxLength = 200;
    private const int GeneratedSlugMaxLength = 190;
    private const int DescriptionMaxLength = 2000;
    private const int CoverImageUrlMaxLength = 2048;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private Game()
    {
    }

    private Game(
        Guid id,
        Guid sellerId,
        string slug,
        string title,
        string? description,
        decimal price,
        DateOnly? releaseDate,
        string? coverImageUrl,
        DateTime createdAt)
    {
        Id = id;
        SellerId = sellerId;
        Slug = slug;
        Title = title;
        Description = description;
        Price = price;
        ReleaseDate = releaseDate;
        CoverImageUrl = coverImageUrl;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid SellerId { get; private set; }

    public string Slug { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public DateOnly? ReleaseDate { get; private set; }

    public string? CoverImageUrl { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Seller? Seller { get; private set; }

    public static Game Create(
        Guid sellerId,
        string title,
        string? description,
        decimal price,
        DateOnly? releaseDate = null,
        string? coverImageUrl = null,
        string? slug = null)
    {
        if (sellerId == Guid.Empty)
        {
            throw new DomainValidationException("A game must belong to a seller.");
        }

        title = DomainText.Required(title, TitleMaxLength);
        slug = NormalizeSlug(slug ?? CreateSlug(title));
        description = DomainText.Optional(description, DescriptionMaxLength);
        coverImageUrl = NormalizeCoverImageUrl(coverImageUrl);

        if (price < 0)
        {
            throw new DomainValidationException("A game price cannot be negative.");
        }

        return new Game(
            Guid.NewGuid(),
            sellerId,
            slug,
            title,
            description,
            price,
            releaseDate,
            coverImageUrl,
            DateTime.UtcNow);
    }

    public void UpdateDetails(
        string title,
        string? description,
        decimal price,
        DateOnly? releaseDate = null,
        string? coverImageUrl = null)
    {
        title = DomainText.Required(title, TitleMaxLength);
        description = DomainText.Optional(description, DescriptionMaxLength);
        coverImageUrl = NormalizeCoverImageUrl(coverImageUrl);

        if (price < 0)
        {
            throw new DomainValidationException("A game price cannot be negative.");
        }

        Title = title;
        Description = description;
        Price = price;
        ReleaseDate = releaseDate;
        CoverImageUrl = coverImageUrl;
    }

    public static string CreateSlug(string title)
    {
        title = DomainText.Required(title, TitleMaxLength);
        var normalizedTitle = title.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var slug = new StringBuilder();
        var separatorPending = false;

        foreach (var character in normalizedTitle)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsAsciiLetterOrDigit(character))
            {
                if (separatorPending && slug.Length > 0)
                {
                    slug.Append('-');
                }

                slug.Append(character);
                separatorPending = false;
            }
            else
            {
                separatorPending = true;
            }

            if (slug.Length >= GeneratedSlugMaxLength)
            {
                break;
            }
        }

        var result = slug.ToString().TrimEnd('-');

        if (result.Length == 0)
        {
            throw new DomainValidationException(
                "A game title must contain letters or numbers to create a product URL.");
        }

        return result;
    }

    private static string NormalizeSlug(string value)
    {
        value = DomainText.Required(value, SlugMaxLength).ToLowerInvariant();

        if (value.Any(character => !char.IsAsciiLetterOrDigit(character) && character != '-')
            || value.StartsWith('-')
            || value.EndsWith('-')
            || value.Contains("--", StringComparison.Ordinal))
        {
            throw new DomainValidationException(
                "A game slug can contain only lowercase letters, numbers, and single hyphens.");
        }

        return value;
    }

    private static string? NormalizeCoverImageUrl(string? value)
    {
        value = DomainText.Optional(value, CoverImageUrlMaxLength);

        if (value is null)
        {
            return null;
        }

        if (value.StartsWith('/') && !value.StartsWith("//", StringComparison.Ordinal))
        {
            return value;
        }

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return value;
        }

        throw new DomainValidationException(
            "A cover image URL must be a root-relative path or an HTTP(S) URL.");
    }
}
