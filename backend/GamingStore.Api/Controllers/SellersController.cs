using Microsoft.AspNetCore.Mvc;

namespace GamingStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SellersController(ISellerService sellerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SellerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var sellers = await sellerService.GetAllAsync(cancellationToken);

        return Ok(sellers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SellerResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var seller = await sellerService.GetByIdAsync(id, cancellationToken);

        return Ok(seller);
    }

    [HttpPost]
    public async Task<ActionResult<SellerResponse>> Create(
        CreateSellerRequest request,
        CancellationToken cancellationToken)
    {
        var seller = await sellerService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = seller.Id }, seller);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SellerResponse>> Update(
        Guid id,
        UpdateSellerRequest request,
        CancellationToken cancellationToken)
    {
        var seller = await sellerService.UpdateAsync(id, request, cancellationToken);

        return Ok(seller);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sellerService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
