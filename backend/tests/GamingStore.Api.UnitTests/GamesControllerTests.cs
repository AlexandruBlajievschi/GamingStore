using GamingStore.Api.Controllers;
using GamingStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamingStore.Api.UnitTests;

public sealed class GamesControllerTests
{
    private static readonly Guid GameId = Guid.Parse("13a7712c-7f0b-4cb3-b9b1-09db733c4c5c");
    private static readonly Guid SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f");
    private static readonly GameResponse Response = new(
        GameId,
        "Starfall Tactics",
        "Fleet strategy.",
        24.99m,
        new DateOnly(2025, 11, 14),
        SellerId,
        "Northbyte Games");

    [Fact]
    public async Task GetAll_ReturnsOkWithGames()
    {
        var controller = new GamesController(new FakeGameService { Games = [Response] });

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var games = Assert.IsAssignableFrom<IReadOnlyList<GameResponse>>(ok.Value);
        Assert.Single(games);
    }

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        var controller = new GamesController(new FakeGameService { Game = Response });

        var result = await controller.GetById(GameId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var controller = new GamesController(new FakeGameService { Game = Response });
        var request = new CreateGameRequest(SellerId, Response.Title, Response.Description, Response.Price, null);

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(GamesController.GetById), created.ActionName);
        Assert.Equal(Response, created.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var controller = new GamesController(new FakeGameService { Game = Response });
        var request = new UpdateGameRequest(Response.Title, Response.Description, Response.Price, null);

        var result = await controller.Update(GameId, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var service = new FakeGameService();
        var controller = new GamesController(service);

        var result = await controller.Delete(GameId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(GameId, service.DeletedGameId);
    }

    private sealed class FakeGameService : IGameService
    {
        public IReadOnlyList<GameResponse> Games { get; init; } = [];

        public GameResponse? Game { get; init; }

        public Guid? DeletedGameId { get; private set; }

        public Task<IReadOnlyList<GameResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Games);
        }

        public Task<GameResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Game ?? Response);
        }

        public Task<GameResponse> CreateAsync(CreateGameRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Game ?? Response);
        }

        public Task<GameResponse> UpdateAsync(
            Guid id,
            UpdateGameRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Game ?? Response);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeletedGameId = id;

            return Task.CompletedTask;
        }
    }
}
