using RpaWorker.Domain.Entities;

namespace RpaWorker.Domain.Interfaces;

public interface IDataParser
{
    public IEnumerable<Price> Parse(string html);
}