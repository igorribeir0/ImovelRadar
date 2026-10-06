using ImovelRadar.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImovelRadar.Api.Controllers;

[ApiController]
[Route("api/market-statistics")]
public class MarketStatisticsController : ControllerBase
{
    private readonly NeighborhoodStatisticsService
        _neighborhoodStatisticsService;

    public MarketStatisticsController(
        NeighborhoodStatisticsService neighborhoodStatisticsService)
    {
        _neighborhoodStatisticsService =
            neighborhoodStatisticsService;
    }

    [HttpGet("neighborhood")]
    public async Task<IActionResult> GetNeighborhoodStatistics(
        [FromQuery] string city,
        [FromQuery] string state,
        [FromQuery] string neighborhood,
        CancellationToken cancellationToken)
    {
        try
        {
            var statistics =
                await _neighborhoodStatisticsService.GetAsync(
                    city,
                    state,
                    neighborhood,
                    cancellationToken);

            return Ok(statistics);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new
            {
                message = exception.Message
            });
        }
    }
}