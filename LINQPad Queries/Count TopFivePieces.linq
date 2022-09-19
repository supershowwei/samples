<Query Kind="Statements">
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Chef.Extensions.Long</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.ComponentModel</Namespace>
</Query>

var dir = @"D:\Applications\MoneyStudio\Quotes";
var volDict = new Dictionary<long, long>();
var bidCountDict = new Dictionary<long, long>();
var askCountDict = new Dictionary<long, long>();
var topFivePieceRatioDict = new Dictionary<decimal, long>();
var prevTopFivePieces = default(TopFivePieces);

foreach (var file in Directory.GetFiles(dir, "*.topfive").OrderByDescending(f => Path.GetFileName(f)).Skip(1))
{
    // 只抓一年內
    if (Path.GetFileNameWithoutExtension(file).CompareTo(DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd")) < 0) break;

    foreach (var line in File.ReadAllLines(file))
    {
        var curtTopFivePieces = TopFivePieces.Deserialize(line);
        
        // 只統計 08:47 之前
        if (curtTopFivePieces.Time >= curtTopFivePieces.Time.Date.Add(TimeSpan.Parse("8:47:0"))) break;
        
		var topBidPieceTotalVolume = curtTopFivePieces.TopBidPieces.Sum(x => x.Volume);
		
        if (!bidCountDict.ContainsKey(topBidPieceTotalVolume))
		{
			bidCountDict.Add(topBidPieceTotalVolume, 0);
		}

		bidCountDict[topBidPieceTotalVolume] = bidCountDict[topBidPieceTotalVolume] + 1;

		var topAskPieceTotalVolume = curtTopFivePieces.TopAskPieces.Sum(x => x.Volume);

		if (!askCountDict.ContainsKey(topAskPieceTotalVolume))
		{
			askCountDict.Add(topAskPieceTotalVolume, 0);
		}

		askCountDict[topAskPieceTotalVolume] = askCountDict[topAskPieceTotalVolume] + 1;
		
		var topFivePieceRatio = topBidPieceTotalVolume >= topAskPieceTotalVolume
			? (decimal)Math.Round(topBidPieceTotalVolume / (double)topAskPieceTotalVolume, 1)
			: -(decimal)Math.Round(topAskPieceTotalVolume / (double)topBidPieceTotalVolume, 1);

		if (!topFivePieceRatioDict.ContainsKey(topFivePieceRatio))
		{
			topFivePieceRatioDict.Add(topFivePieceRatio, 0);
		}
		
		topFivePieceRatioDict[topFivePieceRatio] = topFivePieceRatioDict[topFivePieceRatio] + 1;

		if (prevTopFivePieces != null)
        {
            // 外盤變內盤
            foreach (var askPiece in prevTopFivePieces.TopAskPieces)
            {
                var vol = 0L;

                foreach (var bidPiece in curtTopFivePieces.TopBidPieces)
                {
                    if (askPiece.Price == bidPiece.Price)
                    {
                        vol += askPiece.Volume + bidPiece.Volume;
                        break;
                    }
                }

                if (vol > 0)
                {
                    if (!volDict.ContainsKey(vol))
                    {
                        volDict.Add(vol, 0);
                    }

                    volDict[vol] = volDict[vol] + 1;
                }
            }
            
            // 內盤變外盤
            foreach (var bidPiece in prevTopFivePieces.TopBidPieces)
            {
                var vol = 0L;
                
                foreach (var askPiece in curtTopFivePieces.TopAskPieces)
                {
                    if (bidPiece.Price == askPiece.Price)
                    {
                        vol += (bidPiece.Volume + askPiece.Volume) *-1;
                        break;
                    }
                }
                
                if (vol < 0)
                {
                    if (!volDict.ContainsKey(vol))
                    {
                        volDict.Add(vol, 0);
                    }

                    volDict[vol] = volDict[vol] + 1;
                }
            }
        }

        prevTopFivePieces = curtTopFivePieces;
    }
    
    //break;
}

//bidCountDict.OrderByDescending(kv => kv.Key).Dump();
//askCountDict.OrderByDescending(kv => kv.Key).Dump();
topFivePieceRatioDict.OrderByDescending(kv => kv.Key).Dump();
//volDict.OrderByDescending(kv => kv.Key).Dump();

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