namespace RpaWorker.Domain.Entities;

public class Price
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string Coin { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string FontUrl { get; set; } = string.Empty;
}