<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>System.Globalization</Namespace>
  <Namespace>CsvHelper</Namespace>
  <Namespace>CsvHelper.TypeConversion</Namespace>
  <Namespace>CsvHelper.Configuration.Attributes</Namespace>
  <Namespace>CsvHelper.Configuration</Namespace>
</Query>

var weeklyOptionCandlesticks = new List<Candlestick>();

// 起始 K棒 必須要是某個結算日後 >= 週五交易日
using (var reader = new StreamReader(@"D:\Downloads\candlesticks.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    csv.Context.RegisterClassMap<CandlestickMap>();
    
    var candlesticks = csv.GetRecords<Candlestick>().ToList();

    Candlestick weeklyOptionCandlestick = default;
    DateOnly previousSettlementDate = DateOnly.MinValue;
    DateOnly expectedSettlementDate = DateOnly.MinValue;

    foreach (var candlestick in candlesticks)
    {
        if (expectedSettlementDate == DateOnly.MinValue)
        {
            var daysToAdd = ((int)DayOfWeek.Wednesday - (int)candlestick.FromDate.DayOfWeek + 7) % 7;

            expectedSettlementDate = candlestick.FromDate.AddDays(daysToAdd);
        }

        if (weeklyOptionCandlestick == default)
        {
            if (candlestick.FromDate >= previousSettlementDate)
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
            }
        }
        else
        {
            weeklyOptionCandlestick.ToDate = candlestick.FromDate;
            weeklyOptionCandlestick.High = Math.Max(weeklyOptionCandlestick.High, candlestick.High);
            weeklyOptionCandlestick.Low = Math.Min(weeklyOptionCandlestick.Low, candlestick.Low);
            weeklyOptionCandlestick.Close = candlestick.Close;
        }

        // 大於等於預期的結算日，那就是結算日了。
        if (candlestick.FromDate >= expectedSettlementDate)
        {
            weeklyOptionCandlesticks.Add(weeklyOptionCandlestick);

            weeklyOptionCandlestick = default;
            expectedSettlementDate = DateOnly.MinValue;

            previousSettlementDate = candlestick.FromDate;

            if (previousSettlementDate.DayOfWeek >= DayOfWeek.Wednesday)
            {
                var daysToAdd = ((int)DayOfWeek.Friday - (int)previousSettlementDate.DayOfWeek + 7) % 7;

                previousSettlementDate = previousSettlementDate.AddDays(daysToAdd);
            }
        }
    }
}

weeklyOptionCandlesticks.Dump();
return;

using (var writer = new StreamWriter(@"D:\Downloads\weekly-option-candlesticks.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    var options = new TypeConverterOptions { Formats = new[] { "yyyy-MM-dd" } };

    csv.Context.TypeConverterOptionsCache.AddOptions<DateOnly>(options);

    csv.WriteRecords(weeklyOptionCandlesticks);
}

public class Candlestick
{
    [Name("Date")]
    public DateOnly FromDate { get; set; }
    
    public DateOnly ToDate { get; set; }

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