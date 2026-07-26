using GamingStore.Api.Models;
using GamingStore.Api.Models.Entities;

namespace GamingStore.Api.UnitTests;

public sealed class GameTests
{
    private static readonly Guid SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f");

    [Fact]
    public void Create_NormalizesValidValues()
    {
        var game = Game.Create(SellerId, " New Game ", " Fresh listing. ", 9.99m);

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal(SellerId, game.SellerId);
        Assert.Equal("new-game", game.Slug);
        Assert.Equal("New Game", game.Title);
        Assert.Equal("Fresh listing.", game.Description);
        Assert.Equal(9.99m, game.Price);
        Assert.True(game.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("Auralith Drift", "auralith-drift")]
    [InlineData("  Café 99: Afterlight!  ", "cafe-99-afterlight")]
    public void CreateSlug_ReturnsUrlSafeValue(string title, string expected)
    {
        Assert.Equal(expected, Game.CreateSlug(title));
    }

    [Fact]
    public void Create_StoresValidCoverImageUrl()
    {
        var game = Game.Create(
            SellerId,
            "New Game",
            null,
            9.99m,
            coverImageUrl: " /images/games/new-game.webp ");

        Assert.Equal("/images/games/new-game.webp", game.CoverImageUrl);
    }

    [Fact]
    public void Create_ThrowsValidation_WhenCoverImageUrlIsInvalid()
    {
        Assert.Throws<DomainValidationException>(
            () => Game.Create(
                SellerId,
                "New Game",
                null,
                9.99m,
                coverImageUrl: "images/games/new-game.webp"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenSellerIdIsEmpty()
    {
        Assert.Throws<DomainValidationException>(
            () => Game.Create(Guid.Empty, "New Game", null, 9.99m));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsValidation_WhenTitleIsEmpty(string title)
    {
        Assert.Throws<DomainValidationException>(
            () => Game.Create(SellerId, title, null, 9.99m));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenTitleIsTooLong()
    {
        Assert.Throws<DomainValidationException>(
            () => Game.Create(SellerId, new string('A', 201), null, 9.99m));
    }

    [Fact]
    public void Create_ReturnsNullDescription_WhenDescriptionIsEmpty()
    {
        var game = Game.Create(SellerId, "New Game", "   ", 9.99m);

        Assert.Null(game.Description);
    }

    [Fact]
    public void Create_ThrowsValidation_WhenDescriptionIsTooLong()
    {
        Assert.Throws<DomainValidationException>(
            () => Game.Create(SellerId, "New Game", new string('A', 2001), 9.99m));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenPriceIsNegative()
    {
        Assert.Throws<DomainValidationException>(
            () => Game.Create(SellerId, "New Game", null, -0.01m));
    }

    [Fact]
    public void UpdateDetails_NormalizesAndUpdatesValidValues()
    {
        var game = Game.Create(SellerId, "Old Game", "Old description.", 9.99m);

        game.UpdateDetails(" Updated Game ", " Updated description. ", 12.99m, new DateOnly(2026, 7, 24));

        Assert.Equal("Updated Game", game.Title);
        Assert.Equal("Updated description.", game.Description);
        Assert.Equal(12.99m, game.Price);
        Assert.Equal(new DateOnly(2026, 7, 24), game.ReleaseDate);
        Assert.Equal("old-game", game.Slug);
    }

    [Fact]
    public void UpdateDetails_ReturnsNullDescription_WhenDescriptionIsEmpty()
    {
        var game = Game.Create(SellerId, "Old Game", "Old description.", 9.99m);

        game.UpdateDetails("Updated Game", "   ", 12.99m);

        Assert.Null(game.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_ThrowsValidation_WhenTitleIsEmpty(string title)
    {
        var game = Game.Create(SellerId, "Old Game", "Old description.", 9.99m);

        Assert.Throws<DomainValidationException>(
            () => game.UpdateDetails(title, "Updated description.", 12.99m));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenTitleIsTooLong()
    {
        var game = Game.Create(SellerId, "Old Game", "Old description.", 9.99m);

        Assert.Throws<DomainValidationException>(
            () => game.UpdateDetails(new string('A', 201), "Updated description.", 12.99m));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenDescriptionIsTooLong()
    {
        var game = Game.Create(SellerId, "Old Game", "Old description.", 9.99m);

        Assert.Throws<DomainValidationException>(
            () => game.UpdateDetails("Updated Game", new string('A', 2001), 12.99m));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenPriceIsNegative()
    {
        var game = Game.Create(SellerId, "Old Game", "Old description.", 9.99m);

        Assert.Throws<DomainValidationException>(
            () => game.UpdateDetails("Updated Game", "Updated description.", -0.01m));
    }
}
