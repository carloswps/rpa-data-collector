using Microsoft.Extensions.Options;
using RpaWorker.Domain.Interfaces;

namespace RpaWorker.Application;

public class ScrapingService : IScrapingService
{
    private readonly IDataParser _dataParser;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ScrapingService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
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
            var request = new HttpRequestMessage(HttpMethod.Get, _sourceUrl);
            request.Headers.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36"
            );
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.AcceptLanguage.ParseAdd("pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get data from source.");
                return;
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);

            //var dataPrice = _dataParser.Parse(await response.Content.ReadAsStringAsync(cancellationToken));

            var dataPrice = _dataParser.Parse(html);

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