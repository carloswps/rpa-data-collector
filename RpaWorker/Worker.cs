using Microsoft.Extensions.Options;
using RpaWorker.Application;
using RpaWorker.Domain.Interfaces;

namespace RpaWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ScrapingOptions _options;
    private readonly IScrapingService _scrapingService;

    public Worker(ILogger<Worker> logger, IOptions<ScrapingOptions> options, IScrapingService scrapingService)
    {
        _logger = logger;
        _options = options.Value;
        _scrapingService = scrapingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await _scrapingService.ExecuteAsync(stoppingToken);
                _logger.LogInformation("Worker finished at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromMinutes(_options.IntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException e)
            {
                _logger.LogInformation(e, "Worker stopped.");
            }
    }
}