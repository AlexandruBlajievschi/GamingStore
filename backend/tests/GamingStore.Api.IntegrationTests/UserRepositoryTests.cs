using GamingStore.Api.Models.Entities;
using GamingStore.Api.Repositories;

namespace GamingStore.Api.IntegrationTests;

public sealed class UserRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsSeededUsersOrderedByName()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);

        var users = await repository.GetAllAsync(CancellationToken.None);

        var user = Assert.Single(users);
        Assert.Equal("Alex", user.FirstName);
        Assert.Equal("Player", user.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);
        var userId = Guid.Parse("6d16f5fd-0e50-4e25-894c-5f2d5a767b7f");

        var user = await repository.GetByIdAsync(userId, CancellationToken.None);

        Assert.NotNull(user);
        Assert.Equal("alex.player@gamingstore.local", user.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);

        var user = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetTrackedByIdAsync_ReturnsTrackedUser_WhenUserExists()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);
        var userId = Guid.Parse("6d16f5fd-0e50-4e25-894c-5f2d5a767b7f");

        var user = await repository.GetTrackedByIdAsync(userId, CancellationToken.None);

        Assert.NotNull(user);
        Assert.Contains(
            database.Context.ChangeTracker.Entries<User>(),
            entry => entry.Entity.Id == userId);
    }

    [Fact]
    public async Task GetTrackedByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);

        var user = await repository.GetTrackedByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(user);
    }

    [Fact]
    public async Task AddAsync_AndSaveChangesAsync_PersistUser()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);
        var user = User.Create("Morgan", "Buyer", "morgan.buyer@gamingstore.local");

        await repository.AddAsync(user, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var savedUser = await repository.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(savedUser);
        Assert.Equal("Morgan", savedUser.FirstName);
        Assert.Equal("Buyer", savedUser.LastName);
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsTrackedUserUpdates()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);
        var user = await repository.GetTrackedByIdAsync(Guid.Parse("6d16f5fd-0e50-4e25-894c-5f2d5a767b7f"), CancellationToken.None);
        Assert.NotNull(user);

        user.UpdateDetails("Updated", "Customer", "updated.customer@gamingstore.local");
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var updatedUser = await repository.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.FirstName);
        Assert.Equal("Customer", updatedUser.LastName);
        Assert.Equal("updated.customer@gamingstore.local", updatedUser.Email);
    }

    [Fact]
    public async Task Delete_AndSaveChangesAsync_RemoveUser()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new UserRepository(database.Context);
        var user = User.Create("Delete", "Customer", "delete.customer@gamingstore.local");
        await repository.AddAsync(user, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        repository.Delete(user);
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var deletedUser = await repository.GetByIdAsync(user.Id, CancellationToken.None);

        Assert.Null(deletedUser);
    }
}
