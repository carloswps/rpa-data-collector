using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RpaWorker.Application;
using RpaWorker.Infrastructure.Parsers;

namespace RpaWorker.Tests.Parsers;

public class PriceParserTests
{
    private readonly PriceParser _parser;

    public PriceParserTests()
    {
        var logger = new Mock<ILogger<PriceParser>>();
        var options = new Mock<IOptions<ScrapingOptions>>();
        options.Setup(o => o.Value).Returns(new ScrapingOptions { Url = "http://test" });
        _parser = new PriceParser(logger.Object, options.Object);
    }

    [Fact]
    public void Parse_ValidHtml_ReturnsPriceWithCorrectValue()
    {
        var html = "<div class=\"YMlKec fxKbKc\">5.75</div>";

        var result = _parser.Parse(html).ToList();

        result.Should().HaveCount(1);
        result[0].Value.Should().Be(5.75m);
        result[0].Coin.Should().Be("USD-BRL");
        result[0].FontUrl.Should().Be("http://test");
    }

    [Fact]
    public void Parse_EmptyHtml_ReturnsEmptyList()
    {
        var result = _parser.Parse(string.Empty).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Parse_NullHtml_ReturnsEmptyList()
    {
        var result = _parser.Parse(null!).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Parse_MissingPriceTag_ReturnsEmptyList()
    {
        var html = "<div class=\"outra-classe\">5.75</div>";

        var result = _parser.Parse(html).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void Parse_NonNumericValue_ReturnsEmptyList()
    {
        var html = "<div class=\"YMlKec fxKbKc\">não-é-número</div>";

        var result = _parser.Parse(html).ToList();

        result.Should().BeEmpty();
    }
}
