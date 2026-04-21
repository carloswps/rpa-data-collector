using Microsoft.Extensions.Options;
using RpaWorker.Domain.Interfaces;

namespace RpaWorker.Application;

public class ScrapingService : IScrapingService
{
    private readonly ICollectRepository _collectRepository;
    private readonly IDataParser _dataParser;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ScrapingService> _logger;
    private readonly string _sourceUrl;

    public ScrapingService(ICollectRepository collectRepository, ILogger<ScrapingService> logger,
        IOptions<ScrapingOptions> options, IDataParser dataParser, HttpClient httpClient)
    {
        _collectRepository = collectRepository;
        _logger = logger;
        _sourceUrl = options.Value.Url;
        _dataParser = dataParser;
        _httpClient = httpClient;
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

            foreach (var price in dataPrice)
            {
                await _collectRepository.AddAsync(price);
                _logger.LogInformation("Price added: {Value} {Coin}", price.Value, price.Coin);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error scraping data");
        }
    }
}