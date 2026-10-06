using ImovelRadar.Application.DTOs;
using ImovelRadar.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ImovelRadar.Api.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly PropertyQueryService _propertyQueryService;
    private readonly PropertyAnalysisService _propertyAnalysisService;

    public PropertiesController(
        PropertyQueryService propertyQueryService,
        PropertyAnalysisService propertyAnalysisService)
    {
        _propertyQueryService = propertyQueryService;
        _propertyAnalysisService = propertyAnalysisService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] PropertySearchRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var properties =
                await _propertyQueryService.SearchAsync(
                    request,
                    cancellationToken);

            return Ok(properties);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var property =
                await _propertyQueryService.GetByIdAsync(
                    id,
                    cancellationToken);

            return Ok(property);
        }
        catch (ArgumentOutOfRangeException exception)
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

    [HttpGet("{id:int}/opportunity-analysis")]
    public async Task<IActionResult> GetOpportunityAnalysis(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var analysis =
                await _propertyAnalysisService.AnalyzePropertyAsync(
                    id,
                    cancellationToken);

            return Ok(analysis);
        }
        catch (ArgumentOutOfRangeException exception)
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