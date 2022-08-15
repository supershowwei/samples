<Query Kind="Statements">
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>CsvHelper</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Chef.Extensions.DateTime</Namespace>
  <Namespace>Chef.Extensions.Long</Namespace>
  <Namespace>CsvHelper</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

var dir = @"D:\Applications\MoneyStudio\Quotes";
var dailyCandlestick = default(Candlestick);
var minuteCandlesticks = default(List<Candlestick>);
var mainForce = default(MainForce);
var mainForceQuotes = default(LinkedList<Quote>);
var strategy = default(Strategy);
var profits = new List<Profit>();

foreach (var file in Directory.GetFiles(dir, "*.quote").OrderByDescending(f => Path.GetFileName(f)).Skip(1).Take(20))
{
    dailyCandlestick = default(Candlestick);
    minuteCandlesticks = new List<Candlestick>();
    mainForce = new MainForce();
    mainForceQuotes = new LinkedList<Quote>();
    strategy = new Strategy();

    foreach (var quoteLine in File.ReadAllLines(file))
    {
        // 非期貨不抓
        if (!quoteLine.StartsWith("{\"Symbol\":\"TXF")) continue;

        var quote = JsonConvert.DeserializeObject<Quote>(quoteLine);

        var minuteTime = quote.Time.StopMinute().AddMinutes(1);

        // 新分Ｋ開
        if (minuteCandlesticks.Count > 1 && minuteCandlesticks.Last().Time != minuteTime)
        {
            if ((strategy.OrderPrice.HasValue || strategy.TurningPrice.HasValue) && !strategy.Deal.HasValue)
            {
                strategy.OrderPrice = default;
                strategy.TurningPrice = default;
                strategy.TurningOver = default;
            }
            
            if (quote.Time.ToString("HHmm").CompareTo("0900") >= 0 && quote.Time.ToString("HHmm").CompareTo("0915") <= 0)
            {
                var prevMinK = minuteCandlesticks[minuteCandlesticks.Count - 2];
                var curtMinK = minuteCandlesticks.Last();

                if (curtMinK.IsBullish() && curtMinK.Close < curtMinK.High && quote.Price >= curtMinK.High)
                //if (curtMinK.IsBullish() && curtMinK.Low > prevMinK.Low && curtMinK.High > prevMinK.High && curtMinK.Close < curtMinK.High && quote.Price >= curtMinK.High)
                //if (curtMinK.IsBullish() && curtMinK.Close < curtMinK.High && quote.Price >= curtMinK.High && (quote.Price - curtMinK.Close) >= 2)
                {
                    //Console.WriteLine($"PrevK={(prevMinK.IsBullish() ? "Bullish" : prevMinK.IsBearish() ? "Bearish" : "None")}, Volume={curtMinK.Volume}");
                    strategy.Order(quote.Time, -curtMinK.Close);
                }
                else if (curtMinK.IsBearish() && curtMinK.Close > curtMinK.Low && quote.Price <= curtMinK.Low)
                //else if (curtMinK.IsBearish() && curtMinK.Low < prevMinK.Low && curtMinK.High < prevMinK.High && curtMinK.Close > curtMinK.Low && quote.Price <= curtMinK.Low)
                //else if (curtMinK.IsBearish() && curtMinK.Close > curtMinK.Low && quote.Price <= curtMinK.Low && (quote.Price - curtMinK.Close) <= -2)
                {
                    //Console.WriteLine($"PrevK={(prevMinK.IsBullish() ? "Bullish" : prevMinK.IsBearish() ? "Bearish" : "None")}, Volume={curtMinK.Volume}");
                    strategy.Order(quote.Time, curtMinK.Close);
                }
            }
        }

        UpdateDailyCandlestick(quote);
        UpdateMinuteCandlesticks(minuteTime, quote);
        UpdateMainForce(minuteTime, quote);

        if (strategy.Deal.HasValue)
        {
            strategy.Gain(quote.Price);
        }
        else if (!strategy.OrderPrice.HasValue)
        {
            strategy.Match(quote.Time, quote.Price);
        }
        else if (!strategy.TurningPrice.HasValue)
        {
            strategy.Match(quote.Time, quote.Price);
        }

        // 當沖出場
        if (quote.Time.ToString("HHmmss").CompareTo("131500") >= 0) break;

        // 輸 40 點以上就不玩了
        //if (strategy.Profits.Sum(x => x.Total) <= -40) break;

        // 停利停損 n 次以上就不玩了
        //if (strategy.StoppedCount >= 1) break;
    }

    if (strategy.Deal.HasValue) strategy.ForceStop(dailyCandlestick.Close);
    if (strategy.Profits.Count > 0) profits.AddRange(strategy.Profits);

    if (profits.Count > 0)
    {
        using (var writer = new StreamWriter(@"D:\Downloads\profits.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(profits.OrderBy(x => x.Time));
        }
    }

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

    if (quote.Volume >= MainForce.Threshold)
    {
        mainForceQuotes.AddLast(quote);

        if (!mainForce.Volume.HasValue || quote.Volume > mainForce.Volume)
        {
            mainForce.Price = quote.Price;
            mainForce.Volume = quote.Volume;
        }
    }
}

public class Strategy
{
    public Strategy()
    {
        this.Profits = new List<Profit>();
    }

    public DateTime Time { get; set; }
    public decimal? OrderPrice { get; set; }
    public decimal? TurningPrice { get; set; }
    public decimal? TurningOver { get; set; }
    public decimal? Deal { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? MaxLoss { get; set; }
    public decimal? Breakeven { get; set; }
    public decimal? StopProfit1 { get; set; }
    public decimal? StopProfit2 { get; set; }
    public decimal Profit { get; set; }
    public decimal? MaxProfit { get; set; }
    public int StoppedCount { get; set; }
    public List<Profit> Profits { get; set; }

    public void TurnBack(DateTime time, decimal price)
    {
        if (this.Deal.HasValue) return;

        this.Time = time;
        this.TurningPrice = price;
    }

    public void Order(DateTime time, decimal price)
    {
        if (this.Deal.HasValue) return;

        this.Time = time;
        this.OrderPrice = price;
    }

    public void Match(DateTime time, decimal price)
    {
        if (this.Time > time) return;

        if (this.OrderPrice.HasValue)
        {
            var matchedPrice = price * Math.Sign(this.OrderPrice.Value);

            //if (matchedPrice < this.OrderPrice.Value)
            if (matchedPrice <= this.OrderPrice.Value)
            {
                this.Go(time, this.OrderPrice.Value);
            }
        }

        if (this.TurningPrice.HasValue)
        {
            var matchedPrice = price * Math.Sign(this.TurningPrice.Value);

            if (matchedPrice < this.TurningPrice.Value)
            {
                this.TurningOver = (this.TurningOver ?? 0) + (this.TurningPrice.Value - matchedPrice);
                this.TurningPrice = matchedPrice;
            }
            else if (this.TurningOver.HasValue && (matchedPrice - this.TurningPrice.Value) >= 4)
            {
                this.Go(time, matchedPrice);
            }

            if (this.TurningOver >= 5)
            {
                this.TurningPrice = default;
                this.TurningOver = default;
            }
        }
    }

    public void Go(DateTime time, decimal price)
    {
        if (this.Deal.HasValue) return;

        this.Time = time;
        //this.Deal = price + 2;
        this.Deal = price;

        this.StopLoss = 10;
        //this.Breakeven = 8;
        this.StopProfit1 = 12;
        //this.StopProfit2 = 52;
        this.MaxLoss = 0;
        this.MaxProfit = 0;

        Console.Write($"{time:yyyy-MM-dd HH:mm:ss}, {(price > 0 ? "多" : "空")}, 進場={this.Deal.Value}");
    }

    public void Gain(decimal price)
    {
        if (!this.Deal.HasValue) return;

        this.Profit = (price - Math.Abs(this.Deal.Value)) * Math.Sign(this.Deal.Value);

        if (this.MaxProfit.HasValue)
        {
            this.MaxProfit = Math.Max(this.MaxProfit.Value, this.Profit);
        }

        if (this.MaxLoss.HasValue)
        {
            this.MaxLoss = Math.Min(this.MaxLoss.Value, this.Profit);
        }

        if (this.Breakeven.HasValue && this.Profit >= this.Breakeven.Value)
        {
            this.StopLoss = 0;
            this.Breakeven = default;
        }

        if (this.StopLoss.HasValue && this.Profit <= -this.StopLoss.Value)
        {
            if (this.StopProfit1.HasValue)
            {
                this.Profits.Add(new Profit { Time = this.Time, Deal = this.Deal.Value, Value = this.Profit });
            }

            if (this.StopProfit2.HasValue)
            {
                this.Profits.Add(new Profit { Time = this.Time, Deal = this.Deal.Value, Value = this.Profit });
            }

            this.Stop($", {(this.StopLoss.Value == 0 ? "保本" : "停損")}={(this.StopProfit1.HasValue && this.StopProfit2.HasValue ? this.StopLoss.Value * 2 : -this.StopLoss.Value)}");

            return;
        }

        if (this.StopProfit1.HasValue && this.Profit >= this.StopProfit1.Value)
        {
            this.Profits.Add(new Profit { Time = this.Time, Deal = this.Deal.Value, Value = this.Profit });

            if (this.StopProfit2.HasValue)
            {
                Console.Write($", 利1={this.StopProfit1.Value}");
                this.StopProfit1 = default;
            }
            else
            {
                this.Stop($", 利1={this.StopProfit1.Value}");
                return;
            }
        }

        if (this.StopProfit2.HasValue && this.Profit >= this.StopProfit2.Value)
        {
            this.Profits.Add(new Profit { Time = this.Time, Deal = this.Deal.Value, Value = this.Profit });
            this.Stop($", 利2={this.StopProfit2.Value}");
            return;
        }
    }

    public void ForceStop(decimal price)
    {
        if (!this.Deal.HasValue) return;

        this.Profit = (price - Math.Abs(this.Deal.Value)) * Math.Sign(this.Deal.Value);

        if (this.StopProfit1.HasValue)
        {
            this.Profits.Add(new Profit { Time = this.Time, Deal = this.Deal.Value, Value = this.Profit });
        }

        if (this.StopProfit2.HasValue)
        {
            this.Profits.Add(new Profit { Time = this.Time, Deal = this.Deal.Value, Value = this.Profit });
        }

        this.Stop($", 出場={this.Profit}");
    }

    public void Stop(string result)
    {
        this.OrderPrice = default;
        this.TurningPrice = default;
        this.TurningOver = default;
        this.Deal = default;
        this.StopLoss = default;
        this.Breakeven = default;
        this.StopProfit1 = default;
        this.StopProfit2 = default;
        this.StoppedCount++;

        Console.WriteLine($"{result}, 最大獲利={this.MaxProfit}, 最大回撤={this.MaxLoss}");
    }
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
public class Profit
{
    public DateTime Time { get; set; }
    public decimal Deal { get; set; }
    public decimal Cost { get; set; } = -1.5m;
    public decimal Value { get; set; }
    public decimal Total => this.Value + this.Cost;
}
public class MainForce
{
    public static long Threshold = 30;

    public DateTime Time { get; set; }
    public decimal? Price { get; set; }
    public long? Volume { get; set; }
    public decimal? AveragePrice { get; set; }
}
public class DenseDeal
{
    public DateTime Time { get; set; }
    public long? Volume { get; set; }
}