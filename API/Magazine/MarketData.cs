namespace API.Magazine;

public record MarketRecord
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Movement { get; set; }
    public bool Closed { get; set; }
}

public class MarketData
{
    public List<MarketRecord> Markets { get; set; } = new();
}