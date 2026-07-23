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
}
