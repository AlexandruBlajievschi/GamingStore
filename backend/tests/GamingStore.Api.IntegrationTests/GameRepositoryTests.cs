using GamingStore.Api.Data;
using GamingStore.Api.Models.Entities;
using GamingStore.Api.Repositories;

namespace GamingStore.Api.IntegrationTests;

public sealed class GameRepositoryTests
{
    private static readonly Guid SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f");

    [Fact]
    public async Task GetAllAsync_ReturnsSeededGamesOrderedByTitleWithSeller()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);

        var games = await repository.GetAllAsync(CancellationToken.None);

        Assert.Equal(13, games.Count);
        Assert.Equal(
            [
                "Auralith Drift",
                "Cindervolt Coliseum",
                "Ember Library",
                "Glacier Guild",
                "Glass Planet Survey",
                "Iron Vale Rally",
                "Midnight Parcel",
                "Mosswick & Mooncoin",
                "Neon Orchard",
                "Rune Harbor",
                "Starfall Kitchen",
                "Tiny Titan Tactics",
                "Velvet Circuit"
            ],
            games.Select(game => game.Title));
        Assert.All(games, game => Assert.Equal("Northbyte Games", game.Seller?.Name));
        Assert.All(games, game => Assert.NotNull(game.CoverImageUrl));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsGameWithSeller_WhenGameExists()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);
        var seededGameId = Guid.Parse("13a7712c-7f0b-4cb3-b9b1-09db733c4c5c");

        var game = await repository.GetByIdAsync(seededGameId, CancellationToken.None);

        Assert.NotNull(game);
        Assert.Equal("Auralith Drift", game.Title);
        Assert.Equal("auralith-drift", game.Slug);
        Assert.Equal("/images/games/auralith-drift.webp", game.CoverImageUrl);
        Assert.Equal("Northbyte Games", game.Seller?.Name);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsGameWithSeller_WhenGameExists()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);

        var game = await repository.GetBySlugAsync("auralith-drift", CancellationToken.None);

        Assert.NotNull(game);
        Assert.Equal("Auralith Drift", game.Title);
        Assert.Equal("Northbyte Games", game.Seller?.Name);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsNull_WhenGameDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);

        var game = await repository.GetBySlugAsync("missing-game", CancellationToken.None);

        Assert.Null(game);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenGameDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);

        var game = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(game);
    }

    [Fact]
    public async Task GetTrackedByIdAsync_ReturnsTrackedGame_WhenGameExists()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);
        var seededGameId = Guid.Parse("54acfc5b-694a-42c3-a21c-98188f1cf0a6");

        var game = await repository.GetTrackedByIdAsync(seededGameId, CancellationToken.None);

        Assert.NotNull(game);
        Assert.Contains(
            database.Context.ChangeTracker.Entries<Game>(),
            entry => entry.Entity.Id == seededGameId);
    }

    [Fact]
    public async Task GetTrackedByIdAsync_ReturnsNull_WhenGameDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);

        var game = await repository.GetTrackedByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(game);
    }

    [Fact]
    public async Task SellerExistsAsync_ReturnsExpectedResult()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);

        Assert.True(await repository.SellerExistsAsync(SellerId, CancellationToken.None));
        Assert.False(await repository.SellerExistsAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task AddAsync_AndSaveChangesAsync_PersistGame()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);
        var game = Game.Create(SellerId, "New Game", "Fresh listing.", 29.99m);

        await repository.AddAsync(game, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var savedGame = await repository.GetByIdAsync(game.Id, CancellationToken.None);

        Assert.NotNull(savedGame);
        Assert.Equal("New Game", savedGame.Title);
        Assert.Equal("new-game", savedGame.Slug);
        Assert.Equal("Northbyte Games", savedGame.Seller?.Name);
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsTrackedGameUpdates()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);
        var game = await repository.GetTrackedByIdAsync(Guid.Parse("54acfc5b-694a-42c3-a21c-98188f1cf0a6"), CancellationToken.None);
        Assert.NotNull(game);

        game.UpdateDetails("Updated Arena", "Updated description.", 17.99m, new DateOnly(2026, 4, 5));
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var updatedGame = await repository.GetByIdAsync(game.Id, CancellationToken.None);

        Assert.NotNull(updatedGame);
        Assert.Equal("Updated Arena", updatedGame.Title);
        Assert.Equal("Updated description.", updatedGame.Description);
        Assert.Equal(17.99m, updatedGame.Price);
        Assert.Equal(new DateOnly(2026, 4, 5), updatedGame.ReleaseDate);
    }

    [Fact]
    public async Task Delete_AndSaveChangesAsync_RemoveGame()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new GameRepository(database.Context);
        var game = await repository.GetTrackedByIdAsync(Guid.Parse("ab18590a-11fd-4269-9909-11aa2df97688"), CancellationToken.None);
        Assert.NotNull(game);

        repository.Delete(game);
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var deletedGame = await repository.GetByIdAsync(game.Id, CancellationToken.None);

        Assert.Null(deletedGame);
    }
}
