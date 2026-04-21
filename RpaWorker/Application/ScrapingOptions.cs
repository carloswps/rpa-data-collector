namespace RpaWorker.Application;

public class ScrapingOptions
{
    public string Url { get; set; } = string.Empty;
    public int IntervalMinutes { get; set; }
}