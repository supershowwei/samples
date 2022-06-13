<Query Kind="Statements">
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

var dir = @"D:\Applications\MoneyStudio\Quotes";
var tickDict = new Dictionary<long, long>();
var timeDict = new Dictionary<string, long>();

foreach (var file in Directory.GetFiles(dir, "*.quote").OrderByDescending(f => Path.GetFileName(f)))
{
    // 只抓一年內
    if (Path.GetFileNameWithoutExtension(file).CompareTo(DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")) < 0) break;
    
    foreach (var line in File.ReadAllLines(file))
    {
        // 非期貨不抓
        if (!line.StartsWith("{\"Symbol\":\"TXF")) continue;

        var quote = JsonConvert.DeserializeObject<Quote>(line);
        
        // > 100口的不統計
        if (quote.Volume <= 100)
        {
            if (!tickDict.ContainsKey(quote.Volume))
            {
                tickDict.Add(quote.Volume, 0L);
            }

            tickDict[quote.Volume] = tickDict[quote.Volume] + 1;
        }
        
        var time = quote.Time.ToString("HH:mm");
        
        if (!timeDict.ContainsKey(time))
        {
            timeDict.Add(time, 0L);
        }
        
        timeDict[time] = timeDict[time] + quote.Volume;
    }
    
    //break;
}

tickDict.OrderByDescending(kv => kv.Key).Dump();
timeDict.OrderBy(kv => kv.Key).Dump();

}
public class Quote
{
    public string Symbol { get; set; }
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
    public long Volume { get; set; }
    public long TotalVolume { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalAmount { get; set; }
    public long DealsFromBuy { get; set; }
    public long DealsFromSell { get; set; }
    public long BidVolume { get; set; }
    public long AskVolume { get; set; }
    public OrderVolume OrderVolume { get; set; }
}
public class OrderVolume
{
    public long BidOrders { get; set; }
    public long BidOrderVolume { get; set; }
    public long AskOrders { get; set; }
    public long AskOrderVolume { get; set; }
