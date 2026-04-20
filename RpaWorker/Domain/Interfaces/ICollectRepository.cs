using RpaWorker.Domain.Entities;

namespace RpaWorker.Domain.Interfaces;

public interface ICollectRepository
{
    public Task AddAsync(Price price);
    public Task<IEnumerable<Price>> GetAllAsync();
    public Task<Price?> GetByIdAsync(Guid id);
    public Task<Price?> GetLastAsync();
}