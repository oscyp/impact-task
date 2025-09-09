using Microsoft.AspNetCore.Mvc;
using Tenders.Guru.Facade.Api.Models;
using Tenders.Guru.Facade.Api.Models.DTOs;
using Tenders.Guru.Facade.Api.Models.TenderApiModels;
using Tenders.Guru.Facade.Api.Services;

namespace Tenders.Guru.Facade.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TendersController(ITendersService tendersService, ILogger<TendersController> logger)
    : ControllerBase
{
    private readonly ILogger<TendersController> _logger = logger;

    [HttpGet("{tenderId}")]
    [ProducesResponseType(typeof(TenderDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(int tenderId, CancellationToken cancellationToken)
    {
        var result = await tendersService.GenTender(tenderId, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<TenderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchTenders([FromBody] SearchParams searchParams, CancellationToken cancellationToken)
    {
        var result = await tendersService.SearchTenders(searchParams, cancellationToken);

        return Ok(result);
    }

}