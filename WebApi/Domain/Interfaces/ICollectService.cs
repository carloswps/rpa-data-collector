using rpa_data_collector.DTOs;

namespace rpa_data_collector.Domain.Interfaces;

public interface ICollectService
{
    public Task<IEnumerable<PriceDto>> GetAllAsync();
    public Task<PriceDto?> GetLastAsync();
    public Task<PriceDto?> GetByIdAsync(Guid id);
}