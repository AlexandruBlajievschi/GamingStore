namespace GamingStore.Api.Models.Entities;

public sealed class Game
{
    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 2000;

    private Game()
    {
    }

    private Game(
        Guid id,
        Guid sellerId,
        string title,
        string? description,
        decimal price,
        DateOnly? releaseDate,
        DateTime createdAt)
    {
        Id = id;
        SellerId = sellerId;
        Title = title;
        Description = description;
        Price = price;
        ReleaseDate = releaseDate;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid SellerId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public DateOnly? ReleaseDate { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Seller? Seller { get; private set; }

    public static Game Create(
        Guid sellerId,
        string title,
        string? description,
        decimal price,
        DateOnly? releaseDate = null)
    {
        if (sellerId == Guid.Empty)
        {
            throw new ArgumentException("A game must belong to a seller.", nameof(sellerId));
        }

        title = NormalizeRequiredText(title, nameof(title), TitleMaxLength);
        description = NormalizeOptionalText(description, nameof(description), DescriptionMaxLength);

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), price, "A game price cannot be negative.");
        }

        return new Game(
            Guid.NewGuid(),
            sellerId,
            title,
            description,
            price,
            releaseDate,
            DateTime.UtcNow);
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

    private static string? NormalizeOptionalText(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"A text value cannot be longer than {maxLength} characters.", parameterName);
        }

        return value;
    }
}
