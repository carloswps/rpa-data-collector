using System.Globalization;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using RpaWorker.Application;
using RpaWorker.Domain.Entities;
using RpaWorker.Domain.Interfaces;

namespace RpaWorker.Infrastructure.Parsers;

public class PriceParser : IDataParser
{
    private readonly ILogger<PriceParser> _logger;

    private readonly string _sourceUrl;

    public PriceParser(ILogger<PriceParser> logger,
        IOptions<ScrapingOptions> options)
    {
        _logger = logger;
        _sourceUrl = options.Value.Url;
    }

    public IEnumerable<Price> Parse(string html)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                _logger.LogWarning("Empty HTML received by the parser");
                return [];
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var docNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'YMlKec fxKbKc')]");

            if (docNode is null)
            {
                _logger.LogWarning("No price found in HTML");
                return [];
            }

            if (!decimal.TryParse(docNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                _logger.LogWarning("Failed to convert '{Text}' to decimal.", docNode.InnerText);
                return [];
            }

            return
            [
                new Price
                {
                    Id = Guid.NewGuid(),
                    Value = price,
                    Coin = "USD-BRL",
                    Date = DateTime.UtcNow,
                    FontUrl = _sourceUrl
                }
            ];
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error parsing HTML");
            return [];
        }
    }
}