<Query Kind="Statements">
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Chef.Extensions.DateTime</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

var dir = @"D:\Applications\MoneyStudio\Quotes";
var minuteCandlesticks = new List<Candlestick>();

foreach (var file in Directory.GetFiles(dir, "*.quote").OrderByDescending(f => Path.GetFileName(f)))
{
    foreach (var line in File.ReadAllLines(file))
    {
        // 非期貨不抓
        if (!line.StartsWith("{\"Symbol\":\"TXF")) continue;

        var quote = JsonConvert.DeserializeObject<Quote>(line);

        UpdateMinuteCandlesticks(quote);
    }

    break;
}

void UpdateMinuteCandlesticks(Quote quote)
{
    var time = quote.Time.StopMinute().AddMinutes(1);
    var current = minuteCandlesticks.LastOrDefault();

    if (current == null || current.Time != time)
    {
        current = new Candlestick
        {
            Time = time,
            Open = quote.Price,
            High = quote.Price,
            Low = quote.Price,
            Close = quote.Price,
            Volume = quote.Volume
        };
    }
    else
    {
        current.High = Math.Max(current.High, quote.Price);
        current.Low = Math.Min(current.Low, quote.Price);
        current.Close = quote.Price;
        current.Volume += quote.Volume;
    }
}

}
public class Quote
{
    public string Symbol { get; set; }
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
    public int Volume { get; set; }
    public int TotalVolume { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalAmount { get; set; }
    public int DealsFromBuy { get; set; }
    public int DealsFromSell { get; set; }
    public int BidVolume { get; set; }
    public int AskVolume { get; set; }
    public OrderVolume OrderVolume { get; set; }
}
public class OrderVolume
{
    public int BidOrders { get; set; }
    public int BidOrderVolume { get; set; }
    public int AskOrders { get; set; }
    public int AskOrderVolume { get; set; }
}
public class Candlestick
{
    public DateTime Time { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public long Volume { get; set; }
