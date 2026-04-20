namespace RpaWorker.Domain.Interfaces;

public interface IScrapingService
{
    public Task ExecuteAsync(CancellationToken cancellationToken);
}