using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpa_data_collector.Infrastructure.Persistence;

namespace rpa_data_collector.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;

    public HealthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "1.1.0";

        var canConnect = false;

        try
        {
            canConnect = await _context.Database.CanConnectAsync();
        }
        catch
        {
            // ignored
        }

        var workAlive = false;
        DateTime? lastScraping = null;
        try
        {
            lastScraping = await _context.Prices.MaxAsync(p => (DateTime?)p.Date);

            if (lastScraping.HasValue) workAlive = lastScraping.Value > DateTime.UtcNow.AddMinutes(-15);
        }
        catch
        {
            // ignored
        }

        var isHealthy = canConnect;

        return Ok(new
        {
            status = isHealthy ? "healthy" : "unhealthy",
            version,
            database = canConnect ? "connected" : "disconnected",
            worker = new
            {
                status = workAlive ? "running" : lastScraping.HasValue ? "stopped" : "never_run",
                last_scraping = lastScraping
            },
            timestamp = DateTime.UtcNow,
            envoriment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            machineName = Environment.MachineName
        });
    }
}