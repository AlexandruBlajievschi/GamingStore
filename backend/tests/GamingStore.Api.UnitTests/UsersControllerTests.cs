using GamingStore.Api.Controllers;
using GamingStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamingStore.Api.UnitTests;

public sealed class UsersControllerTests
{
    private static readonly Guid UserId = Guid.Parse("6d16f5fd-0e50-4e25-894c-5f2d5a767b7f");
    private static readonly UserResponse Response = new(
        UserId,
        "Alex",
        "Player",
        "alex.player@gamingstore.local",
        new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc));

    [Fact]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        var controller = new UsersController(new FakeUserService { Users = [Response] });

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsAssignableFrom<IReadOnlyList<UserResponse>>(ok.Value);
        Assert.Single(users);
    }

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        var controller = new UsersController(new FakeUserService { User = Response });

        var result = await controller.GetById(UserId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var controller = new UsersController(new FakeUserService { User = Response });
        var request = new CreateUserRequest(Response.FirstName, Response.LastName, Response.Email);

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(UsersController.GetById), created.ActionName);
        Assert.Equal(Response, created.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var controller = new UsersController(new FakeUserService { User = Response });
        var request = new UpdateUserRequest(Response.FirstName, Response.LastName, Response.Email);

        var result = await controller.Update(UserId, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var service = new FakeUserService();
        var controller = new UsersController(service);

        var result = await controller.Delete(UserId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(UserId, service.DeletedUserId);
    }

    private sealed class FakeUserService : IUserService
    {
        public IReadOnlyList<UserResponse> Users { get; init; } = [];

        public UserResponse? User { get; init; }

        public Guid? DeletedUserId { get; private set; }

        public Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Users);
        }

        public Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(User ?? Response);
        }

        public Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(User ?? Response);
        }

        public Task<UserResponse> UpdateAsync(
            Guid id,
            UpdateUserRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(User ?? Response);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeletedUserId = id;

            return Task.CompletedTask;
        }
    }
}
