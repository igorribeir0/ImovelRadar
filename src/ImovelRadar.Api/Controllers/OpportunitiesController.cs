using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImovelRadar.Api.Controllers;

[ApiController]
[Route("api/opportunities")]
public class OpportunitiesController : ControllerBase
{
    private readonly OpportunityRankingService
        _opportunityRankingService;

    public OpportunitiesController(
        OpportunityRankingService opportunityRankingService)
    {
        _opportunityRankingService =
            opportunityRankingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRanking(
        [FromQuery] OpportunityRankingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var ranking =
                await _opportunityRankingService
                    .GetRankingAsync(
                        request,
                        cancellationToken);

            return Ok(ranking);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(
                new
                {
                    message = exception.Message
                });
        }
    }
}