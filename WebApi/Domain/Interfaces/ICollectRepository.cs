using rpa_data_collector.Domain.Entities;

namespace rpa_data_collector.Domain.Interfaces;

public interface ICollectRepository
{
    public Task AddAsync(Price price);
    public Task<IEnumerable<Price>> GetAllAsync();
    public Task<Price?> GetByIdAsync(Guid id);
    public Task<Price?> GetLastAsync();
}