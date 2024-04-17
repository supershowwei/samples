<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>CsvHelper.Configuration.Attributes</Namespace>
  <Namespace>CsvHelper.TypeConversion</Namespace>
  <Namespace>CsvHelper.Configuration</Namespace>
</Query>

var monthlyOptionCandlesticks = new List<Candlestick>();

// 起始 K棒 必須要是某個結算日後的第一個交易日
using (var reader = new StreamReader(@"D:\Downloads\candlesticks.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    csv.Context.RegisterClassMap<CandlestickMap>();

    var candlesticks = csv.GetRecords<Candlestick>().ToList();

    Candlestick monthlyOptionCandlestick = default;
    DateTime expectedSettlementDate = default;

    foreach (var candlestick in candlesticks)
    {
        if (expectedSettlementDate == default)
        {
            expectedSettlementDate = WeekdayCalculator.GetThirdWednesday(candlestick.FromDate);
        }

        if (monthlyOptionCandlestick == default)
        {
            if (candlestick.FromDate.DayOfWeek == DayOfWeek.Thursday)
            {
                monthlyOptionCandlestick = new Candlestick
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
            monthlyOptionCandlestick.ToDate = candlestick.FromDate;
            monthlyOptionCandlestick.TradeDates++;
            monthlyOptionCandlestick.High = Math.Max(monthlyOptionCandlestick.High, candlestick.High);
            monthlyOptionCandlestick.Low = Math.Min(monthlyOptionCandlestick.Low, candlestick.Low);
            monthlyOptionCandlestick.Close = candlestick.Close;
        }

        // 大於等於預期的結算日，那就是結算日了。
        if (candlestick.FromDate >= expectedSettlementDate)
        {
            if (monthlyOptionCandlestick?.TradeDates > 1)
            {
                monthlyOptionCandlesticks.Add(monthlyOptionCandlestick);
            }

            monthlyOptionCandlestick = default;
            expectedSettlementDate = default;
        }
    }
}

monthlyOptionCandlesticks = monthlyOptionCandlesticks.TakeLast(100).ToList();

monthlyOptionCandlesticks.Dump();
return;

using (var writer = new StreamWriter(@"D:\Downloads\monthly-option-candlesticks.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    var options = new TypeConverterOptions { Formats = new[] { "yyyy-MM-dd" } };

    csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);

    csv.WriteRecords(monthlyOptionCandlesticks.Select(x => new
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

public class WeekdayCalculator
{
    public static DateTime GetThirdWednesday(DateTime currentDate)
    {
        var thirdWednesday = GetNthWeekdayOfMonth(currentDate.Year, currentDate.Month, DayOfWeek.Wednesday, 3);

        // 如果當前日期在當月第三個星期三之後，則計算下個月的第三個星期三
        if (currentDate > thirdWednesday)
        {
            var nextMonth = currentDate.AddMonths(1);
            
            thirdWednesday = GetNthWeekdayOfMonth(nextMonth.Year, nextMonth.Month, DayOfWeek.Wednesday, 3);
        }

        return thirdWednesday;
    }

    private static DateTime GetNthWeekdayOfMonth(int year, int month, DayOfWeek dayOfWeek, int nth)
    {
        var firstDayOfMonth = new DateTime(year, month, 1);
        var daysOffset = (int)dayOfWeek - (int)firstDayOfMonth.DayOfWeek;

        if (daysOffset < 0)
        {
            daysOffset += 7;
        }

        var firstWeekday = firstDayOfMonth.AddDays(daysOffset);
        
        return firstWeekday.AddDays((nth - 1) * 7);
    }
}