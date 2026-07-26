using GamingStore.Api.DTOs;
using GamingStore.Api.Models;
using GamingStore.Api.Models.Entities;
using GamingStore.Api.Repositories;
using GamingStore.Api.Services;

namespace GamingStore.Api.UnitTests;

public sealed class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedUsers()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");
        var service = new UserService(FakeUserRepository.WithUsers(user));

        var users = await service.GetAllAsync(CancellationToken.None);

        var response = Assert.Single(users);
        Assert.Equal(user.Id, response.Id);
        Assert.Equal("Alex", response.FirstName);
        Assert.Equal("Player", response.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedUser_WhenUserExists()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");
        var service = new UserService(FakeUserRepository.WithUsers(user));

        var response = await service.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.Equal(user.Id, response.Id);
        Assert.Equal("alex.player@gamingstore.local", response.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenUserDoesNotExist()
    {
        var service = new UserService(new FakeUserRepository());

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_AddsUserAndSaves_WhenRequestIsValid()
    {
        var repository = new FakeUserRepository();
        var service = new UserService(repository);
        var request = new CreateUserRequest(" Alex ", " Player ", " ALEX.PLAYER@GAMINGSTORE.LOCAL ");

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("Alex", response.FirstName);
        Assert.Equal("Player", response.LastName);
        Assert.Equal("alex.player@gamingstore.local", response.Email);
        Assert.Equal(1, repository.SaveChangesCount);
        Assert.Contains(repository.Users, user => user.Id == response.Id);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidation_WhenRequestViolatesDomainRules()
    {
        var repository = new FakeUserRepository();
        var service = new UserService(repository);
        var request = new CreateUserRequest("", "Player", "not-an-email");

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.CreateAsync(request, CancellationToken.None));

        Assert.Empty(repository.Users);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ChangesUserAndSaves_WhenUserExists()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");
        var repository = FakeUserRepository.WithUsers(user);
        var service = new UserService(repository);
        var request = new UpdateUserRequest("Morgan", "Buyer", "morgan.buyer@gamingstore.local");

        var response = await service.UpdateAsync(user.Id, request, CancellationToken.None);

        Assert.Equal("Morgan", response.FirstName);
        Assert.Equal("Buyer", response.LastName);
        Assert.Equal("morgan.buyer@gamingstore.local", response.Email);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenUserDoesNotExist()
    {
        var service = new UserService(new FakeUserRepository());
        var request = new UpdateUserRequest("Missing", "User", "missing@gamingstore.local");

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsValidationAndDoesNotMutate_WhenRequestViolatesDomainRules()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");
        var repository = FakeUserRepository.WithUsers(user);
        var service = new UserService(repository);
        var request = new UpdateUserRequest("", "Buyer", "bad-email");

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateAsync(user.Id, request, CancellationToken.None));

        Assert.Equal("Alex", user.FirstName);
        Assert.Equal("Player", user.LastName);
        Assert.Equal("alex.player@gamingstore.local", user.Email);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUserAndSaves_WhenUserExists()
    {
        var user = User.Create("Delete", "User", "delete.user@gamingstore.local");
        var repository = FakeUserRepository.WithUsers(user);
        var service = new UserService(repository);

        await service.DeleteAsync(user.Id, CancellationToken.None);

        Assert.Empty(repository.Users);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFound_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var service = new UserService(repository);

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.DeleteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(0, repository.SaveChangesCount);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public int SaveChangesCount { get; private set; }

        public static FakeUserRepository WithUsers(params User[] users)
        {
            var repository = new FakeUserRepository();
            repository.Users.AddRange(users);

            return repository;
        }

        public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<User>>(Users);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
        }

        public Task<User?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
        }

        public Task AddAsync(User user, CancellationToken cancellationToken)
        {
            Users.Add(user);

            return Task.CompletedTask;
        }

        public void Delete(User user)
        {
            Users.Remove(user);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCount++;

            return Task.CompletedTask;
        }
    }
}
