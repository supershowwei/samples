<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>System.Globalization</Namespace>
  <Namespace>CsvHelper</Namespace>
  <Namespace>CsvHelper.TypeConversion</Namespace>
  <Namespace>CsvHelper.Configuration.Attributes</Namespace>
  <Namespace>CsvHelper.Configuration</Namespace>
</Query>

var weeklyOptionCandlesticks = new List<Candlestick>();

// 起始 K棒 必須要是某個結算日
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

        // 大於等於預期的結算日，那就是結算日了。
        if (candlestick.FromDate >= expectedSettlementDate)
        {
            weeklyOptionCandlestick = new Candlestick
            {
                FromDate = candlestick.FromDate,
                ToDate = candlestick.FromDate,
                Open = candlestick.Open,
                High = candlestick.High,
                Low = candlestick.Low,
                Close = candlestick.Close
            };
            
            weeklyOptionCandlesticks.Add(weeklyOptionCandlestick);
            
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

    csv.WriteRecords(weeklyOptionCandlesticks);
}

public class Candlestick
{
    [Name("Date")]
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

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
    }
}