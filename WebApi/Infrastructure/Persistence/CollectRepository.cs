using Microsoft.EntityFrameworkCore;
using rpa_data_collector.Domain.Entities;
using rpa_data_collector.Domain.Interfaces;

namespace rpa_data_collector.Infrastructure.Persistence;

public class CollectRepository : ICollectRepository
{
    private readonly AppDbContext _context;

    public CollectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Price price)
    {
        await _context.Prices.AddAsync(price);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Price>> GetAllAsync()
    {
        return await _context.Prices.ToListAsync();
    }

    public async Task<Price?> GetByIdAsync(Guid id)
    {
        return await _context.Prices.FindAsync(id);
    }

    public Task<Price?> GetLastAsync()
    {
        return _context.Prices.OrderByDescending(p => p.Date).FirstOrDefaultAsync();
    }
}