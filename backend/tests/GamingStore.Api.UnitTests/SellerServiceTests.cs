using GamingStore.Api.Models;
using GamingStore.Api.Models.Entities;
using GamingStore.Api.Repositories;
using GamingStore.Api.Services;

namespace GamingStore.Api.UnitTests;

public sealed class SellerServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsMappedSellers()
    {
        var seller = Seller.Create("Northbyte Games", "studio@northbyte.local", "Indie studio.");
        var service = new SellerService(FakeSellerRepository.WithSellers(seller));

        var sellers = await service.GetAllAsync(CancellationToken.None);

        var response = Assert.Single(sellers);
        Assert.Equal(seller.Id, response.Id);
        Assert.Equal("Northbyte Games", response.Name);
        Assert.Equal("studio@northbyte.local", response.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedSeller_WhenSellerExists()
    {
        var seller = Seller.Create("Northbyte Games", "studio@northbyte.local");
        var service = new SellerService(FakeSellerRepository.WithSellers(seller));

        var response = await service.GetByIdAsync(seller.Id, CancellationToken.None);

        Assert.Equal(seller.Id, response.Id);
        Assert.Equal("Northbyte Games", response.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ThrowsNotFound_WhenSellerDoesNotExist()
    {
        var service = new SellerService(new FakeSellerRepository());

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_AddsSellerAndSaves_WhenRequestIsValid()
    {
        var repository = new FakeSellerRepository();
        var service = new SellerService(repository);
        var request = new CreateSellerRequest(" Northbyte Games ", " STUDIO@NORTHBYTE.LOCAL ", " Indie studio. ");

        var response = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("Northbyte Games", response.Name);
        Assert.Equal("studio@northbyte.local", response.Email);
        Assert.Equal(1, repository.SaveChangesCount);
        Assert.Contains(repository.Sellers, seller => seller.Id == response.Id);
    }

    [Fact]
    public async Task CreateAsync_ThrowsValidation_WhenRequestViolatesDomainRules()
    {
        var repository = new FakeSellerRepository();
        var service = new SellerService(repository);
        var request = new CreateSellerRequest("", "not-an-email", null);

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.CreateAsync(request, CancellationToken.None));

        Assert.Empty(repository.Sellers);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ChangesSellerAndSaves_WhenSellerExists()
    {
        var seller = Seller.Create("Old Seller", "old@seller.local");
        var repository = FakeSellerRepository.WithSellers(seller);
        var service = new SellerService(repository);
        var request = new UpdateSellerRequest("New Seller", "new@seller.local", "Updated.");

        var response = await service.UpdateAsync(seller.Id, request, CancellationToken.None);

        Assert.Equal("New Seller", response.Name);
        Assert.Equal("new@seller.local", response.Email);
        Assert.Equal("Updated.", response.Description);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFound_WhenSellerDoesNotExist()
    {
        var service = new SellerService(new FakeSellerRepository());
        var request = new UpdateSellerRequest("Missing", "missing@seller.local", null);

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsValidationAndDoesNotMutate_WhenRequestViolatesDomainRules()
    {
        var seller = Seller.Create("Original", "original@seller.local", "Original.");
        var repository = FakeSellerRepository.WithSellers(seller);
        var service = new SellerService(repository);
        var request = new UpdateSellerRequest("", "bad-email", "Changed.");

        await Assert.ThrowsAsync<DomainValidationException>(
            () => service.UpdateAsync(seller.Id, request, CancellationToken.None));

        Assert.Equal("Original", seller.Name);
        Assert.Equal("original@seller.local", seller.Email);
        Assert.Equal(0, repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_RemovesSellerAndSaves_WhenSellerExists()
    {
        var seller = Seller.Create("Delete Seller", "delete@seller.local");
        var repository = FakeSellerRepository.WithSellers(seller);
        var service = new SellerService(repository);

        await service.DeleteAsync(seller.Id, CancellationToken.None);

        Assert.Empty(repository.Sellers);
        Assert.Equal(1, repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFound_WhenSellerDoesNotExist()
    {
        var repository = new FakeSellerRepository();
        var service = new SellerService(repository);

        await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => service.DeleteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(0, repository.SaveChangesCount);
    }

    private sealed class FakeSellerRepository : ISellerRepository
    {
        public List<Seller> Sellers { get; } = [];

        public int SaveChangesCount { get; private set; }

        public static FakeSellerRepository WithSellers(params Seller[] sellers)
        {
            var repository = new FakeSellerRepository();
            repository.Sellers.AddRange(sellers);

            return repository;
        }

        public Task<IReadOnlyList<Seller>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Seller>>(Sellers);
        }

        public Task<Seller?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Sellers.FirstOrDefault(seller => seller.Id == id));
        }

        public Task<Seller?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Sellers.FirstOrDefault(seller => seller.Id == id));
        }

        public Task AddAsync(Seller seller, CancellationToken cancellationToken)
        {
            Sellers.Add(seller);

            return Task.CompletedTask;
        }

        public void Delete(Seller seller)
        {
            Sellers.Remove(seller);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCount++;

            return Task.CompletedTask;
        }
    }
}
