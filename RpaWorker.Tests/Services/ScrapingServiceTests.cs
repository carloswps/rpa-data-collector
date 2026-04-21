using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RpaWorker.Application;
using RpaWorker.Domain.Entities;
using RpaWorker.Domain.Interfaces;
using System.Net;

namespace RpaWorker.Tests.Services;

public class ScrapingServiceTests
{
    private readonly Mock<IDataParser> _parserMock;
    private readonly Mock<ICollectRepository> _repositoryMock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<ILogger<ScrapingService>> _loggerMock;
    private readonly IOptions<ScrapingOptions> _options;

    public ScrapingServiceTests()
    {
        _parserMock = new Mock<IDataParser>();
        _repositoryMock = new Mock<ICollectRepository>();
        _loggerMock = new Mock<ILogger<ScrapingService>>();
        _options = Options.Create(new ScrapingOptions { Url = "http://test", IntervalMinutes = 10 });

        var scope = new Mock<IServiceScope>();
        scope.Setup(s => s.ServiceProvider.GetService(typeof(ICollectRepository)))
            .Returns(_repositoryMock.Object);

        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scope.Object);
    }

    private ScrapingService BuildService(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);
        return new ScrapingService(httpClient, _parserMock.Object, _scopeFactoryMock.Object, _loggerMock.Object, _options);
    }

    [Fact]
    public async Task ExecuteAsync_SuccessfulResponse_SavesPrice()
    {
        var price = new Price { Id = Guid.NewGuid(), Value = 5.75m, Coin = "USD-BRL", Date = DateTime.UtcNow, FontUrl = "http://test" };
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns([price]);

        var service = BuildService(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<html>") });
        await service.ExecuteAsync(CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(price), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_HttpFailure_DoesNotSave()
    {
        var service = BuildService(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        await service.ExecuteAsync(CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Price>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ParserReturnsEmpty_DoesNotSave()
    {
        _parserMock.Setup(p => p.Parse(It.IsAny<string>())).Returns([]);

        var service = BuildService(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("<html>") });
        await service.ExecuteAsync(CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Price>()), Times.Never);
    }
}
