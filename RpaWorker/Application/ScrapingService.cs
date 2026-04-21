using Microsoft.Extensions.Options;
using RpaWorker.Domain.Interfaces;

namespace RpaWorker.Application;

public class ScrapingService : IScrapingService
{
    private readonly HttpClient _httpClient;
    private readonly IDataParser _dataParser;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScrapingService> _logger;
    private readonly string _sourceUrl;

    public ScrapingService(
        HttpClient httpClient,
        IDataParser dataParser,
        IServiceScopeFactory scopeFactory,
        ILogger<ScrapingService> logger,
        IOptions<ScrapingOptions> options)
    {
        _httpClient = httpClient;
        _dataParser = dataParser;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _sourceUrl = options.Value.Url;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(_sourceUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get data from source.");
                return;
            }

            var dataPrice = _dataParser.Parse(await response.Content.ReadAsStringAsync(cancellationToken));

            if (!dataPrice.Any())
            {
                _logger.LogWarning("Parser returned empty result.");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICollectRepository>();

            foreach (var price in dataPrice)
            {
                await repository.AddAsync(price);
                _logger.LogInformation("Price added: {Value} {Coin}", price.Value, price.Coin);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error scraping data");
        }
    }
}