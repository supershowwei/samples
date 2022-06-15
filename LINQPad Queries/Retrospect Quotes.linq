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
var topPieces = default(Dictionary<decimal, TopPiece>);
var bigOrderTopPieces = default(Dictionary<decimal, TopPiece>);
var bigOrderTopPieceJournal = default(Queue<BigOrderTopPieceHauntArgs>);
var mainForce = default(MainForce);
var strategy = default(Strategy);
var tsmc499 = default(Quote);
var profits = new List<Profit>();

foreach (var file in Directory.GetFiles(dir, "*.quote").OrderByDescending(f => Path.GetFileName(f)).Skip(1).Take(2))
{
	var topFivePiecesFile = Path.ChangeExtension(file, "topfive");

	if (!File.Exists(topFivePiecesFile)) break;

	// 列出𡘙委價出現消失的歷程
	topPieces = new Dictionary<decimal, TopPiece>();
	bigOrderTopPieces = new Dictionary<decimal, TopPiece>();
	bigOrderTopPieceJournal = new Queue<UserQuery.BigOrderTopPieceHauntArgs>();

	foreach (var topFivePieceLine in File.ReadAllLines(topFivePiecesFile))
	{
		var topFivePieces = TopFivePieces.Deserialize(topFivePieceLine);

		GenerateBigTopPieceJournal(topFivePieces);
	}
    
    bigOrderTopPieceJournal.Dump();
    return;

	dailyCandlestick = default(Candlestick);
	minuteCandlesticks = new List<Candlestick>();
	mainForce = new MainForce();
	strategy = new Strategy();
	tsmc499 = default(Quote);

	var bigOrderTopPiece = default(BigOrderTopPieceHauntArgs);

	foreach (var quoteLine in File.ReadAllLines(file))
	{
		if (quoteLine.StartsWith("{\"Symbol\":\"2330") && tsmc499 == null)
		{
			var tsmcQuote = JsonConvert.DeserializeObject<Quote>(quoteLine);

			if (tsmcQuote.Volume == 499) tsmc499 = tsmcQuote;

			continue;
		}

		// 非期貨不抓
		if (!quoteLine.StartsWith("{\"Symbol\":\"TXF")) continue;

		var quote = JsonConvert.DeserializeObject<Quote>(quoteLine);

		// 處理𡘙委價
		var nextBigOrderTopPiece = bigOrderTopPieceJournal.Count > 0 ? bigOrderTopPieceJournal.Peek() : default(BigOrderTopPieceHauntArgs);

		if (nextBigOrderTopPiece != null && quote.Time >= nextBigOrderTopPiece.Time)
		{
			if (nextBigOrderTopPiece.IsVisible)
			{
				bigOrderTopPiece = bigOrderTopPieceJournal.Dequeue();
			}
			else
			{
				bigOrderTopPieceJournal.Dequeue();
				bigOrderTopPiece = null;
			}
		}

		if (bigOrderTopPiece != null && quote.Time >= bigOrderTopPiece.Time.StopMinute().AddMinutes(1))
		{
			if (bigOrderTopPiece.TopPiece.Side == OrderSide.Buy && quote.Price <= bigOrderTopPiece.TopPiece.Price)
			{
				strategy.Go(quote.Time, quote.Price);
				bigOrderTopPiece= null;
			}
			else if (bigOrderTopPiece.TopPiece.Side == OrderSide.Sell && quote.Price >= bigOrderTopPiece.TopPiece.Price)
			{
				strategy.Go(quote.Time, -quote.Price);
				bigOrderTopPiece= null;
			}
		}

		var minuteTime = quote.Time.StopMinute().AddMinutes(1);

		// 新分Ｋ開
		if (minuteCandlesticks.Count > 1 && minuteCandlesticks.Last().Time != minuteTime)
		{
			var minuteCandlestick = minuteCandlesticks.Last();
		}

		// 台積電 499
		//		if (tsmc499 != null)
		//		{
		//			if (tsmc499.AskVolume == 499)
		//			{
		//				strategy.Go(quote.Time, quote.Price);
		//			}
		//			else if (tsmc499.BidVolume == 499)
		//			{
		//				strategy.Go(quote.Time, -quote.Price);
		//			}
		//
		//			tsmc499 = default(Quote);
		//		}

		if (strategy.Deal.HasValue) strategy.Gain(quote.Price);

		UpdateDailyCandlestick(quote);
		UpdateMinuteCandlesticks(minuteTime, quote);
		UpdateMainForce(minuteTime, quote);

		// 當沖出場
		if (quote.Time >= quote.Time.Date.Add(TimeSpan.Parse("13:14:0"))) break;

		// 輸 40 點以上就不玩了
		//if (strategy.Profits.Sum(x => x.Total) <= -40) break;
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

	if (quote.Volume > MainForce.Threshold || quote.Volume > mainForce.Volume)
	{
		mainForce.Price = quote.Price;
		mainForce.Volume = quote.Volume;
		mainForce.High = mainForce.High.HasValue ? Math.Max(quote.Price, mainForce.High.Value) : quote.Price;
		mainForce.Low = mainForce.Low.HasValue ? Math.Min(quote.Price, mainForce.Low.Value) : quote.Price;
	}
}

void GenerateBigTopPieceJournal(TopFivePieces topFivePieces)
{
	for (var i = 0; i < 5; i++)
	{
		if (i < topFivePieces.TopBidPieces.Count)
		{
			DetectBigOrderTopPiece(topFivePieces.TopBidPieces[i], topFivePieces.Time);
		}

		if (i < topFivePieces.TopAskPieces.Count)
		{
			DetectBigOrderTopPiece(topFivePieces.TopAskPieces[i], topFivePieces.Time);
		}
	}

	TryRemoveBigOrderTopPiece(topFivePieces);

	void DetectBigOrderTopPiece(TopPiece topPiece, DateTime time)
	{
		if (topPieces.TryGetValue(topPiece.Price, out var prevTopPiece))
		{
			topPiece.Delta = prevTopPiece.Side == topPiece.Side
								 ? topPiece.Volume - prevTopPiece.Volume
								 : topPiece.Volume + prevTopPiece.Volume;
		}
        else
        {
            topPiece.Delta = topPiece.Volume;
        }

		if (topPiece.Delta >= 100)
		{
			bigOrderTopPieces[topPiece.Price] = topPiece;

			bigOrderTopPieceJournal.Enqueue(new BigOrderTopPieceHauntArgs(time, new TopPiece(topPiece.Side, topPiece.Price, topPiece.Volume) { Delta = topPiece.Delta }, true));
		}

		topPieces[topPiece.Price] = topPiece;
	}
}

void TryRemoveBigOrderTopPiece(TopFivePieces topFivePieces)
{
	var lastTopBidPieces = topFivePieces.TopBidPieces.Last();
	var lastTopAskPieces = topFivePieces.TopAskPieces.Last();

	foreach (var bigOrderPrice in bigOrderTopPieces.Keys.ToArray())
	{
		var bigOrderTopPiece = bigOrderTopPieces[bigOrderPrice];

		if (bigOrderTopPiece.Side == OrderSide.Buy && lastTopAskPieces.Price < bigOrderTopPiece.Price)
		{
			Remove(bigOrderTopPiece, topFivePieces.Time);
		}
		else if (bigOrderTopPiece.Side == OrderSide.Sell && lastTopBidPieces.Price > bigOrderTopPiece.Price)
		{
			Remove(bigOrderTopPiece, topFivePieces.Time);
		}
	}

	void Remove(TopPiece bigOrderTopPiece, DateTime time)
	{
		bigOrderTopPieces.Remove(bigOrderTopPiece.Price);

		bigOrderTopPieceJournal.Enqueue(new BigOrderTopPieceHauntArgs(time, new TopPiece(bigOrderTopPiece.Side, bigOrderTopPiece.Price, bigOrderTopPiece.Volume) { Delta = bigOrderTopPiece.Delta }, false));
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
	public decimal? Deal { get; set; }
	public decimal? StopLoss { get; set; }
	public decimal? MaxStopLoss { get; set; }
	public decimal? Breakeven { get; set; }
	public decimal? StopProfit1 { get; set; }
	public decimal? StopProfit2 { get; set; }
	public decimal Profit { get; set; }
	public List<Profit> Profits { get; set; }

	public void Go(DateTime time, decimal price)
	{
		if (this.Deal.HasValue) return;

		this.Time = time;
		this.Deal = price + 2;

		this.StopLoss = 10;

		// 躲牆後
		if (this.StopLoss.HasValue)
		{
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
		}

		this.Breakeven = 10;
		this.StopProfit1 = 12;
		this.StopProfit2 = 52;
		this.MaxStopLoss = 0;

		Console.Write($"{time:yyyy-MM-dd HH:mm:ss}, {(price > 0 ? "多" : "空")}, 進場={this.Deal.Value}");
	}

	public void Gain(decimal price)
	{
		if (!this.Deal.HasValue) return;

		this.Profit = (price - Math.Abs(this.Deal.Value)) * Math.Sign(this.Deal.Value);

		if (this.MaxStopLoss.HasValue)
		{
			this.MaxStopLoss = Math.Min(this.MaxStopLoss.Value, this.Profit);
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

			this.Stop($", {(this.StopLoss.Value == 0 ? "保本" : "停損")}={this.StopLoss.Value * 2}");

			return;
		}

		if (this.StopProfit1.HasValue && this.Profit >= this.StopProfit1.Value)
		{
			this.Profits.Add(new Profit { Time = this.Time, Deal = this.Deal.Value, Value = this.Profit });
			Console.Write($", 利1={this.StopProfit1.Value}");
			this.StopProfit1 = default;
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
		this.Deal = default;
		this.StopLoss = default;
		this.Breakeven = default;
		this.StopProfit1 = default;
		this.StopProfit2 = default;

		Console.WriteLine($"{result}, 最大回撤={this.MaxStopLoss}");
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
	public decimal? High { get; set; }
	public decimal? Low { get; set; }
}
public enum OrderSide
{
	[Description("買")]
	Buy,

	[Description("賣")]
	Sell
}
public class TopPiece
{
	public TopPiece(OrderSide side, decimal price, long volume)
	{
		this.Side = side;
		this.Price = price;
		this.Volume = volume;
	}

	public OrderSide Side { get; }

	public decimal Price { get; }

	public long Volume { get; }

	public long Delta { get; set; }
}
public class TopFivePieces
{
	private static readonly Regex PieceRegex = new Regex(@"([\d\-\.]+):([\d\-\.]+)", RegexOptions.Compiled);

	public TopFivePieces(string symbol, DateTime time, IReadOnlyList<TopPiece> topBidPieces, IReadOnlyList<TopPiece> topAskPieces)
	{
		this.Symbol = symbol;
		this.Time = time;
		this.TopBidPieces = topBidPieces;
		this.TopAskPieces = topAskPieces;
	}

	public string Symbol { get; }

	public DateTime Time { get; }

	public IReadOnlyList<TopPiece> TopBidPieces { get; }

	public IReadOnlyList<TopPiece> TopAskPieces { get; }

	public static TopFivePieces Deserialize(string value)
	{
		var arr = value.Split('_');

		var symbol = arr[0];
		var time = long.Parse(arr[1]).ToDateTime();

		var topBidPieces = PieceRegex.Matches(arr[2])
			.OfType<Match>()
			.Select(m => new TopPiece(OrderSide.Buy, decimal.Parse(m.Groups[1].Value), long.Parse(m.Groups[2].Value)))
			.ToList();

		var topAskPieces = PieceRegex.Matches(arr[3])
			.OfType<Match>()
			.Select(m => new TopPiece(OrderSide.Sell, decimal.Parse(m.Groups[1].Value), long.Parse(m.Groups[2].Value)))
			.ToList();

		return new TopFivePieces(symbol, time, topBidPieces, topAskPieces);
	}
}
public class BigOrderTopPieceHauntArgs
{
	public BigOrderTopPieceHauntArgs(DateTime time, TopPiece topPiece, bool isVisible)
	{
		this.Time = time;
		this.TopPiece = topPiece;
		this.IsVisible = isVisible;
	}

	public DateTime Time { get; }

	public TopPiece TopPiece { get; }

	public bool IsVisible { get; }