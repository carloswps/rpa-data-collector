using rpa_data_collector.Domain.Interfaces;
using rpa_data_collector.DTOs;

namespace rpa_data_collector.Application.Services;

public class CollectService : ICollectService
{
    private readonly ICollectRepository _collectRepository;

    public CollectService(ICollectRepository collectRepository)
    {
        _collectRepository = collectRepository;
    }

    public async Task<IEnumerable<PriceDto>> GetAllAsync()
    {
        var prices = await _collectRepository.GetAllAsync();
        var res = prices.Select(p => new PriceDto
        {
            Id = p.Id,
            Date = p.Date,
            Coin = p.Coin,
            Value = p.Value,
            FontUrl = p.FontUrl,
            PercentageChange = p.PercentageChange
        });
        return res;
    }

    public async Task<PriceDto?> GetLastAsync()
    {
        var prices = await _collectRepository.GetLastAsync();
        if (prices is null) return null;
        var res = new PriceDto
        {
            Id = prices.Id,
            Date = prices.Date,
            Coin = prices.Coin,
            Value = prices.Value,
            PercentageChange = prices.PercentageChange,
            FontUrl = prices.FontUrl
        };
        return res;
    }

    public async Task<PriceDto?> GetByIdAsync(Guid id)
    {
        var prices = await _collectRepository.GetByIdAsync(id);
        if (prices is null) return null;

        var res = new PriceDto
        {
            Id = prices.Id,
            Coin = prices.Coin,
            Date = prices.Date,
            Value = prices.Value,
            PercentageChange = prices.PercentageChange,
            FontUrl = prices.FontUrl
        };
        return res;
    }
}
