<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>CsvHelper.Configuration.Attributes</Namespace>
  <Namespace>CsvHelper.TypeConversion</Namespace>
  <Namespace>CsvHelper.Configuration</Namespace>
</Query>

var weeklyOptionCandlesticks = new List<Candlestick>();

// 起始 K棒 必須要是某個結算日後的第一個交易日
using (var reader = new StreamReader(@"D:\Downloads\candlesticks.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    csv.Context.RegisterClassMap<CandlestickMap>();

    var candlesticks = csv.GetRecords<Candlestick>().ToList();

    Candlestick weeklyOptionCandlestick = default;
    DateTime expectedSettlementDate = default;

    foreach (var candlestick in candlesticks)
    {
        if (expectedSettlementDate == default)
        {
            var daysToAdd = ((int)DayOfWeek.Wednesday - (int)candlestick.FromDate.DayOfWeek + 7) % 7;

            expectedSettlementDate = candlestick.FromDate.AddDays(daysToAdd);
        }

        if (weeklyOptionCandlestick == default)
        {
            if (candlestick.FromDate.DayOfWeek == DayOfWeek.Thursday)
            {
                weeklyOptionCandlestick = new Candlestick
                {
                    FromDate = candlestick.FromDate,
                    ToDate = candlestick.FromDate,
                    TradeDates = 1,
                    Open = candlestick.Open,
                    High = candlestick.High,
                    Low = candlestick.Low,
                    Close = candlestick.Close
                };
            }
        }
        else
        {
            weeklyOptionCandlestick.ToDate = candlestick.FromDate;
            weeklyOptionCandlestick.TradeDates++;
            weeklyOptionCandlestick.High = Math.Max(weeklyOptionCandlestick.High, candlestick.High);
            weeklyOptionCandlestick.Low = Math.Min(weeklyOptionCandlestick.Low, candlestick.Low);
            weeklyOptionCandlestick.Close = candlestick.Close;
        }

        // 大於等於預期的結算日，那就是結算日了。
        if (candlestick.FromDate >= expectedSettlementDate)
        {
            if (weeklyOptionCandlestick?.TradeDates > 1)
            {
                weeklyOptionCandlesticks.Add(weeklyOptionCandlestick);
            }

            weeklyOptionCandlestick = default;
            expectedSettlementDate = default;
        }
    }
}

weeklyOptionCandlesticks = weeklyOptionCandlesticks.TakeLast(100).ToList();

weeklyOptionCandlesticks.Dump();
return;

using (var writer = new StreamWriter(@"D:\Downloads\weekly-option-candlesticks.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    var options = new TypeConverterOptions { Formats = new[] { "yyyy-MM-dd" } };

    csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);

    csv.WriteRecords(weeklyOptionCandlesticks.Select(x => new
    {
        x.FromDate,
        x.ToDate,
        x.Open,
        x.High,
        x.Low,
        x.Close
    }));
}

public class Candlestick
{
    [Name("Date")]
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int TradeDates { get; set; }

    public decimal Open { get; set; }

    public decimal High { get; set; }

    public decimal Low { get; set; }

    public decimal Close { get; set; }
}

public sealed class CandlestickMap : ClassMap<Candlestick>
{
    public CandlestickMap()
    {
        this.AutoMap(CultureInfo.InvariantCulture);
        this.Map(m => m.ToDate).Ignore();
        this.Map(m => m.TradeDates).Ignore();
    }
}