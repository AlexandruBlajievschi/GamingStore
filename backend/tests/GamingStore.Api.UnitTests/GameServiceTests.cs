using GamingStore.Api.DTOs;
using GamingStore.Api.Models;
using GamingStore.Api.Models.Entities;
using GamingStore.Api.Repositories;
using GamingStore.Api.Services;

namespace GamingStore.Api.UnitTests;

public sealed class GameServiceTests
{
    private static readonly Guid SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f");

    [Fact]
    public async Task GetAllAsync_ReturnsMappedGames()
    {
        var game = Game.Create(SellerId, "Starfall Tactics", "Fleet strategy.", 24.99m);
        var repository = FakeGameRepository.WithGames(game);
        var service = new GameService(repository);

        var games = await service.GetAllAsync(CancellationToken.None);

        var response = Assert.Single(games);
        Assert.Equal(game.Id, response.Id);
        Assert.Equal("starfall-tactics", response.Slug);
        Assert.Equal("Starfall Tactics", response.Title);
        Assert.Equal("Fleet strategy.", response.Description);
        Assert.Equal(24.99m, response.Price);
        Assert.Equal(SellerId, response.SellerId);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedGame_WhenGameExists()
    {
        var game = Game.Create(SellerId, "Dungeon Ledger", null, 19.99m);
        var repository = FakeGameRepository.WithGames(game);
        var service = new GameService(repository);

        var response = await service.GetByIdAsync(game.Id, CancellationToken.None);

        Assert.Equal(game.Id, response.Id);
        Assert.Equal("Dungeon Ledger", response.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenGameDoesNotExist()
    {
        var service = new GameService(new FakeGameRepository());

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsMappedGame_WhenGameExists()
    {
        var game = Game.Create(SellerId, "Auralith Drift", null, 24.99m);
        var service = new GameService(FakeGameRepository.WithGames(game));

        var response = await service.GetBySlugAsync("auralith-drift", CancellationToken.None);

        Assert.Equal(game.Id, response.Id);
        Assert.Equal("auralith-drift", response.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_ThrowsNotFound_WhenGameDoesNotExist()
    {
        var service = new GameService(new FakeGameRepository());

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.GetBySlugAsync("missing-game", CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_AddsGameAndSaves_WhenRequestIsValid()
    {
        var repository = new FakeGameRepository();
        repository.AddSeller(SellerId);
        var service = new GameService(repository);
        var request = new CreateGameRequest(
            SellerId,
            " Pixel Forge Arena ",
            " Fast battles. ",
            14.99m,
            null,
            "/images/games/pixel-forge-arena.webp");

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("pixel-forge-arena", response.Slug);
        Assert.Equal("Pixel Forge Arena", response.Title);
        Assert.Equal("Fast battles.", response.Description);
        Assert.Equal(14.99m, response.Price);
        Assert.Equal("/images/games/pixel-forge-arena.webp", response.CoverImageUrl);
        Assert.Equal(1, repository.SaveChangesCount);
        Assert.Contains(repository.Games, game => game.Id == response.Id);
    }

    [Fact]
    public async Task CreateAsync_AppendsSuffix_WhenGeneratedSlugAlreadyExists()
    {
        var existingGame = Game.Create(SellerId, "Auralith Drift", null, 24.99m);
        var repository = FakeGameRepository.WithGames(existingGame);
        var service = new GameService(repository);
        var request = new CreateGameRequest(SellerId, "Auralith Drift", null, 29.99m, null);

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("auralith-drift-2", response.Slug);
    }

    [Fact]
    public async Task CreateAsync_ThrowsNotFound_WhenSellerDoesNotExist()
    {
        var repository = new FakeGameRepository();
        var service = new GameService(repository);
        var request = new CreateGameRequest(Guid.NewGuid(), "Unknown Seller Game", null, 9.99m, null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.CreateAsync(request, CancellationToken.None));

        Assert.Empty(repository.Games);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidation_WhenRequestViolatesDomainRules()
    {
        var repository = new FakeGameRepository();
        repository.AddSeller(SellerId);
        var service = new GameService(repository);
        var request = new CreateGameRequest(SellerId, "", null, -1m, null);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.CreateAsync(request, CancellationToken.None));

        Assert.Empty(repository.Games);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ChangesGameAndSaves_WhenGameExists()
    {
        var game = Game.Create(SellerId, "Old Title", "Old description.", 10m);
        var repository = FakeGameRepository.WithGames(game);
        var service = new GameService(repository);
        var request = new UpdateGameRequest("New Title", "New description.", 12.50m, new DateOnly(2026, 1, 2));

        var response = await service.UpdateAsync(game.Id, request, CancellationToken.None);

        Assert.Equal(game.Id, response.Id);
        Assert.Equal("New Title", response.Title);
        Assert.Equal("New description.", response.Description);
        Assert.Equal(12.50m, response.Price);
        Assert.Equal(new DateOnly(2026, 1, 2), response.ReleaseDate);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenGameDoesNotExist()
    {
        var service = new GameService(new FakeGameRepository());
        var request = new UpdateGameRequest("Missing", null, 12m, null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsValidationAndDoesNotMutate_WhenRequestViolatesDomainRules()
    {
        var game = Game.Create(SellerId, "Original", "Original description.", 10m);
        var repository = FakeGameRepository.WithGames(game);
        var service = new GameService(repository);
        var request = new UpdateGameRequest("", "Changed description.", -2m, null);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateAsync(game.Id, request, CancellationToken.None));

        Assert.Equal("Original", game.Title);
        Assert.Equal("Original description.", game.Description);
        Assert.Equal(10m, game.Price);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_RemovesGameAndSaves_WhenGameExists()
    {
        var game = Game.Create(SellerId, "Delete Me", null, 4.99m);
        var repository = FakeGameRepository.WithGames(game);
        var service = new GameService(repository);

        await service.DeleteAsync(game.Id, CancellationToken.None);

        Assert.Empty(repository.Games);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFound_WhenGameDoesNotExist()
    {
        var repository = new FakeGameRepository();
        var service = new GameService(repository);

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.DeleteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(0, repository.SaveChangesCount);
    }

    private sealed class FakeGameRepository : IGameRepository
    {
        private readonly HashSet<Guid> _sellerIds = [SellerId];

        public List<Game> Games { get; } = [];

        public int SaveChangesCount { get; private set; }

        public static FakeGameRepository WithGames(params Game[] games)
        {
            var repository = new FakeGameRepository();
            repository.Games.AddRange(games);

            return repository;
        }

        public void AddSeller(Guid sellerId)
        {
            _sellerIds.Add(sellerId);
        }

        public Task<IReadOnlyList<Game>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Game>>(Games);
        }

        public Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Games.FirstOrDefault(game => game.Id == id));
        }

        public Task<Game?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
        {
            return Task.FromResult(Games.FirstOrDefault(game => game.Slug == slug));
        }

        public Task<Game?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Games.FirstOrDefault(game => game.Id == id));
        }

        public Task<bool> SellerExistsAsync(Guid sellerId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_sellerIds.Contains(sellerId));
        }

        public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
        {
            return Task.FromResult(Games.Any(game => game.Slug == slug));
        }

        public Task AddAsync(Game game, CancellationToken cancellationToken)
        {
            Games.Add(game);

            return Task.CompletedTask;
        }

        public void Delete(Game game)
        {
            Games.Remove(game);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCount++;

            return Task.CompletedTask;
        }
    }
}
