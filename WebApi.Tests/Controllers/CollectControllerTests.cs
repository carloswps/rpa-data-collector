using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using rpa_data_collector.Controllers;
using rpa_data_collector.Domain.Interfaces;
using rpa_data_collector.DTOs;

namespace WebApi.Tests.Controllers;

public class CollectControllerTests
{
    private readonly Mock<ICollectService> _serviceMock;
    private readonly CollectController _controller;

    public CollectControllerTests()
    {
        _serviceMock = new Mock<ICollectService>();
        _controller = new CollectController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOkWithList()
    {
        var prices = new List<PriceDto>
        {
            new() { Id = Guid.NewGuid(), Value = 5.75m, Coin = "USD-BRL" }
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(prices);

        var result = await _controller.GetAllAsync();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(prices);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var price = new PriceDto { Id = id, Value = 5.75m, Coin = "USD-BRL" };
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(price);

        var result = await _controller.GetByIdAsync(id);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(price);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PriceDto?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetLatestAsync_WhenDataExists_ReturnsOk()
    {
        var price = new PriceDto { Id = Guid.NewGuid(), Value = 5.75m, Coin = "USD-BRL" };
        _serviceMock.Setup(s => s.GetLastAsync()).ReturnsAsync(price);

        var result = await _controller.GetLatestAsync();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(price);
    }

    [Fact]
    public async Task GetLatestAsync_WhenNoData_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetLastAsync()).ReturnsAsync((PriceDto?)null);

        var result = await _controller.GetLatestAsync();

        result.Should().BeOfType<NotFoundResult>();
    }
}
