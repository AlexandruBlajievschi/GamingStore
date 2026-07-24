using GamingStore.Api.Controllers;
using GamingStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamingStore.Api.UnitTests;

public sealed class SellersControllerTests
{
    private static readonly Guid SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f");
    private static readonly SellerResponse Response = new(
        SellerId,
        "Northbyte Games",
        "studio@northbyte.local",
        "Indie studio.",
        new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc));

    [Fact]
    public async Task GetAll_ReturnsOkWithSellers()
    {
        var controller = new SellersController(new FakeSellerService { Sellers = [Response] });

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var sellers = Assert.IsAssignableFrom<IReadOnlyList<SellerResponse>>(ok.Value);
        Assert.Single(sellers);
    }

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        var controller = new SellersController(new FakeSellerService { Seller = Response });

        var result = await controller.GetById(SellerId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        var controller = new SellersController(new FakeSellerService { Seller = Response });
        var request = new CreateSellerRequest(Response.Name, Response.Email, Response.Description);

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(SellersController.GetById), created.ActionName);
        Assert.Equal(Response, created.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var controller = new SellersController(new FakeSellerService { Seller = Response });
        var request = new UpdateSellerRequest(Response.Name, Response.Email, Response.Description);

        var result = await controller.Update(SellerId, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(Response, ok.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var service = new FakeSellerService();
        var controller = new SellersController(service);

        var result = await controller.Delete(SellerId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(SellerId, service.DeletedSellerId);
    }

    private sealed class FakeSellerService : ISellerService
    {
        public IReadOnlyList<SellerResponse> Sellers { get; init; } = [];

        public SellerResponse? Seller { get; init; }

        public Guid? DeletedSellerId { get; private set; }

        public Task<IReadOnlyList<SellerResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Sellers);
        }

        public Task<SellerResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Seller ?? Response);
        }

        public Task<SellerResponse> CreateAsync(CreateSellerRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Seller ?? Response);
        }

        public Task<SellerResponse> UpdateAsync(
            Guid id,
            UpdateSellerRequest request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(Seller ?? Response);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            DeletedSellerId = id;

            return Task.CompletedTask;
        }
    }
}
