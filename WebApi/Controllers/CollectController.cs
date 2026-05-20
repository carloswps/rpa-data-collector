using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using rpa_data_collector.Domain.Interfaces;

namespace rpa_data_collector.Controllers;

[ApiController]
[Authorize]
[EnableRateLimiting("fixed")]
[Route("api/v1/collect")]
public class CollectController : ControllerBase
{
    private readonly ICollectService _collectService;

    public CollectController(ICollectService collectService)
    {
        _collectService = collectService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _collectService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _collectService.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestAsync()
    {
        var result = await _collectService.GetLastAsync();
        if (result is null) return NotFound();
        return Ok(result);
    }
}
