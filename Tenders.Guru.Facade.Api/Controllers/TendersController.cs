using Microsoft.AspNetCore.Mvc;
using Tenders.Guru.Facade.Api.Services;

namespace Tenders.Guru.Facade.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TendersController : ControllerBase
{
    private readonly ITendersApiService _tendersApiService;
    private readonly ILogger<TendersController> _logger;

    public TendersController(ITendersApiService tendersApiService , ILogger<TendersController> logger)
    {
        _tendersApiService = tendersApiService;
        _logger = logger;
    }

    [HttpGet("{tenderId}")]
    [ProducesResponseType(typeof(TenderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(int tenderId, CancellationToken cancellationToken)
    {
        var result = await _tendersApiService.GenTender(tenderId, cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(TendersResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTenders([FromQuery] SearchParams searchParams, CancellationToken cancellationToken)
    {
        var result = await _tendersApiService.GetTenders(searchParams, cancellationToken);

        return Ok(result);
    }
}