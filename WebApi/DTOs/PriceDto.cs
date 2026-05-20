namespace rpa_data_collector.DTOs;

public class PriceDto
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public decimal? PercentageChange { get; set; }
    public string Coin { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string FontUrl { get; set; } = string.Empty;
}
