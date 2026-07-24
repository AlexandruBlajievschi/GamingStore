using Microsoft.AspNetCore.Mvc;

namespace GamingStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GamesController(IGameService gameService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GameResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var games = await gameService.GetAllAsync(cancellationToken);

        return Ok(games);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GameResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var game = await gameService.GetByIdAsync(id, cancellationToken);

        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<GameResponse>> Create(
        CreateGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = await gameService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GameResponse>> Update(
        Guid id,
        UpdateGameRequest request,
        CancellationToken cancellationToken)
    {
        var game = await gameService.UpdateAsync(id, request, cancellationToken);

        return Ok(game);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await gameService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
