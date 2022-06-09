<Query Kind="Statements">
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Chef.Extensions.DateTime</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

var dir = @"D:\Applications\MoneyStudio\Quotes";
var dailyCandlestick = default(Candlestick);
var minuteCandlesticks = default(List<Candlestick>);
var mainForce = default(MainForce);
var strategy = default(Strategy);

foreach (var file in Directory.GetFiles(dir, "*.quote").OrderByDescending(f => Path.GetFileName(f)).Take(10))
{
    dailyCandlestick = default(Candlestick);
    minuteCandlesticks = new List<Candlestick>();
    mainForce = new MainForce();
    strategy = new Strategy();

    foreach (var line in File.ReadAllLines(file))
    {
        // 非期貨不抓
        if (!line.StartsWith("{\"Symbol\":\"TXF")) continue;

        var quote = JsonConvert.DeserializeObject<Quote>(line);

        var minuteTime = quote.Time.StopMinute().AddMinutes(1);

        // 新分Ｋ開
        if (minuteCandlesticks.Count > 1 && minuteCandlesticks.Last().Time != minuteTime)
        {
            var minuteCandlestick = minuteCandlesticks.Last();

            //  && quote.Time < quote.Time.Date.Add(TimeSpan.Parse("11:0:0"))
            if (mainForce.Price == mainForce.High && minuteCandlestick.High == dailyCandlestick.High && mainForce.Price <= minuteCandlestick.High && mainForce.Price > minuteCandlestick.Open && mainForce.Price > minuteCandlestick.Close)
            {
                if (!strategy.Deal.HasValue) strategy.Go(quote.Time, -quote.Price);
            }
            else if (mainForce.Price == mainForce.Low && minuteCandlestick.Low == dailyCandlestick.Low && mainForce.Price >= minuteCandlestick.Low && mainForce.Price < minuteCandlestick.Open && mainForce.Price < minuteCandlestick.Close)
            {
                if (!strategy.Deal.HasValue) strategy.Go(quote.Time, quote.Price);
            }
        }

        if (strategy.Deal.HasValue) strategy.Gain(quote.Price);

        UpdateDailyCandlestick(quote);
        UpdateMinuteCandlesticks(minuteTime, quote);
        UpdateMainForce(minuteTime, quote);
    }

    if (strategy.Deal.HasValue) Console.WriteLine();

    //break;
}

void UpdateDailyCandlestick(Quote quote)
{
    if (dailyCandlestick == null)
    {
        dailyCandlestick = new Candlestick
        {
            Time = quote.Time.Date,
            Open = quote.Price,
            High = quote.Price,
            Low = quote.Price,
            Close = quote.Price,
            Volume = quote.Volume
        };
    }
    else
    {
        dailyCandlestick.High = Math.Max(dailyCandlestick.High, quote.Price);
        dailyCandlestick.Low = Math.Min(dailyCandlestick.Low, quote.Price);
        dailyCandlestick.Close = quote.Price;
        dailyCandlestick.Volume += quote.Volume;
    }
}

void UpdateMinuteCandlesticks(DateTime time, Quote quote)
{
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

        minuteCandlesticks.Add(current);
    }
    else
    {
        current.High = Math.Max(current.High, quote.Price);
        current.Low = Math.Min(current.Low, quote.Price);
        current.Close = quote.Price;
        current.Volume += quote.Volume;
    }
}

void UpdateMainForce(DateTime time, Quote quote)
{
    if (mainForce.Time != time)
    {
        mainForce = new MainForce { Time = time };
    }

    if (quote.Volume > MainForce.Threshold || quote.Volume > mainForce.Volume)
    {
        mainForce.Price = quote.Price;
        mainForce.Volume = quote.Volume;
        mainForce.High = mainForce.High.HasValue ? Math.Max(quote.Price, mainForce.High.Value) : quote.Price;
        mainForce.Low = mainForce.Low.HasValue ? Math.Min(quote.Price, mainForce.Low.Value) : quote.Price;
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

    // 多方Ｋ棒
    public bool IsBullish()
    {
        return (this.Close - this.Low) > (this.High - this.Close);
    }

    // 空方Ｋ棒
    public bool IsBearish()
    {
        return (this.High - this.Close) > (this.Close - this.Low);
    }
}
public class MainForce
{
    public static long Threshold = 30;

    public DateTime Time { get; set; }
    public decimal? Price { get; set; }
    public long? Volume { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
}
public class Strategy
{
    public DateTime Time { get; set; }
    public decimal? Deal { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? Breakeven { get; set; }
    public decimal? StopProfit1 { get; set; }
    public decimal? StopProfit2 { get; set; }

    public void Go(DateTime time, decimal price)
    {
        this.Deal = price + 2;

        // 躲牆後
        this.StopLoss = 10;
        switch ((this.Deal.Value - this.StopLoss.Value) % 10)
        {
            case 0:
            case 5:
            case -5:
                this.StopLoss += 1;
                break;
            case 1:
            case -9:
            case 6:
            case -4:
                this.StopLoss += 2;
                break;
        }

        this.Breakeven = 10;
        this.StopProfit1 = 12;
        this.StopProfit2 = 52;

        Console.Write($"{time:yyyy-MM-dd HH:mm:ss}, 進場={this.Deal.Value}");
    }

    public void Gain(decimal price)
    {
        if (!this.Deal.HasValue) return;

        var profit = (price - Math.Abs(this.Deal.Value)) * Math.Sign(this.Deal.Value);

        if (this.Breakeven.HasValue && profit >= this.Breakeven.Value)
        {
            this.StopLoss = 0;
            this.Breakeven = default;
        }

        if (this.StopLoss.HasValue && profit <= -this.StopLoss.Value)
        {
            this.Stop($", {(this.StopLoss.Value == 0 ? "保本" : "停損")}={this.StopLoss.Value * 2}");
            return;
        }

        if (this.StopProfit1.HasValue && profit >= this.StopProfit1.Value)
        {
            Console.Write($", 利1={this.StopProfit1.Value}");
            this.StopProfit1 = default;
        }

        if (this.StopProfit2.HasValue && profit >= this.StopProfit2.Value)
        {
            this.Stop($", 利2={this.StopProfit2.Value}");
            return;
        }
    }

    public void Stop(string result)
    {
        this.Deal = default;
        this.StopLoss = default;
        this.Breakeven = default;
        this.StopProfit1 = default;
        this.StopProfit2 = default;

        Console.WriteLine(result);
    }
