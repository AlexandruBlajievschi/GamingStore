using GamingStore.Api.Models.Entities;
using GamingStore.Api.Repositories;

namespace GamingStore.Api.IntegrationTests;

public sealed class SellerRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsSeededSellersOrderedByName()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);

        var sellers = await repository.GetAllAsync(CancellationToken.None);

        var seller = Assert.Single(sellers);
        Assert.Equal("Northbyte Games", seller.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSeller_WhenSellerExists()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);
        var sellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f");

        var seller = await repository.GetByIdAsync(sellerId, CancellationToken.None);

        Assert.NotNull(seller);
        Assert.Equal("Northbyte Games", seller.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenSellerDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);

        var seller = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(seller);
    }

    [Fact]
    public async Task GetTrackedByIdAsync_ReturnsTrackedSeller_WhenSellerExists()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);
        var sellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f");

        var seller = await repository.GetTrackedByIdAsync(sellerId, CancellationToken.None);

        Assert.NotNull(seller);
        Assert.Contains(
            database.Context.ChangeTracker.Entries<Seller>(),
            entry => entry.Entity.Id == sellerId);
    }

    [Fact]
    public async Task GetTrackedByIdAsync_ReturnsNull_WhenSellerDoesNotExist()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);

        var seller = await repository.GetTrackedByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(seller);
    }

    [Fact]
    public async Task AddAsync_AndSaveChangesAsync_PersistSeller()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);
        var seller = Seller.Create("New Studio", "new@studio.local", "Fresh seller.");

        await repository.AddAsync(seller, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var savedSeller = await repository.GetByIdAsync(seller.Id, CancellationToken.None);

        Assert.NotNull(savedSeller);
        Assert.Equal("New Studio", savedSeller.Name);
        Assert.Equal("new@studio.local", savedSeller.Email);
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsTrackedSellerUpdates()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);
        var seller = await repository.GetTrackedByIdAsync(Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), CancellationToken.None);
        Assert.NotNull(seller);

        seller.UpdateDetails("Updated Studio", "updated@studio.local", "Updated seller.");
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var updatedSeller = await repository.GetByIdAsync(seller.Id, CancellationToken.None);

        Assert.NotNull(updatedSeller);
        Assert.Equal("Updated Studio", updatedSeller.Name);
        Assert.Equal("updated@studio.local", updatedSeller.Email);
        Assert.Equal("Updated seller.", updatedSeller.Description);
    }

    [Fact]
    public async Task Delete_AndSaveChangesAsync_RemoveSeller()
    {
        await using var database = await TestDatabase.CreateAsync();
        var repository = new SellerRepository(database.Context);
        var seller = Seller.Create("Delete Studio", "delete@studio.local");
        await repository.AddAsync(seller, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        repository.Delete(seller);
        await repository.SaveChangesAsync(CancellationToken.None);

        database.Context.ChangeTracker.Clear();
        var deletedSeller = await repository.GetByIdAsync(seller.Id, CancellationToken.None);

        Assert.Null(deletedSeller);
    }
}
