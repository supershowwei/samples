<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>CsvHelper.Configuration.Attributes</Namespace>
</Query>

var weeklyOptionCandlesticks = new List<Candlestick>();

// 起始 K棒 要是某結算日後的第一個交易日
using (var reader = new StreamReader(@"D:\Downloads\candlesticks.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    var candlesticks = csv.GetRecords<Candlestick>().ToList();

    //records.Dump();

    Candlestick weeklyOptionCandlestick = default;
    
    DateTime settlementDate = default;

    foreach (var candlestick in candlesticks)
    {
        // 星期三，一定是結算日。
        if (candlestick.WeekDay == DayOfWeek.Wednesday)
        {
            
        }
        
        // 星期三不交易，下一個交易日，即結算日。
        
        // 跨週
        if (candlestick.DiffWeekDays < 0 || candlestick.DiffDays >= 7)
        {
            weeklyOptionCandlestick = candlestick;
            weeklyOptionCandlestick.NextTradeDate = string.Empty;
            weeklyOptionCandlestick.NextOpen = decimal.MinValue;
            weeklyOptionCandlestick.NextHigh = decimal.MinValue;
            weeklyOptionCandlestick.NextLow = decimal.MaxValue;

            continue;
        }

        // 結算日
        if (weeklyOptionCandlestick != null)
        {
            if (weeklyOptionCandlestick.NextOpen < 0)
            {
                weeklyOptionCandlestick.NextOpen = candlestick.PrevOpen;
            }

            weeklyOptionCandlestick.NextTradeDate = candlestick.PrevTradeDate;
            weeklyOptionCandlestick.NextHigh = Math.Max(weeklyOptionCandlestick.NextHigh, candlestick.PrevHigh);
            weeklyOptionCandlestick.NextLow = Math.Min(weeklyOptionCandlestick.NextLow, candlestick.PrevLow);
            weeklyOptionCandlestick.NextClose = candlestick.PrevClose;

            if (weeklyOptionCandlestick.WeekDay < 3 || candlestick.WeekDay == 3 || (weeklyOptionCandlestick.WeekDay + weeklyOptionCandlestick.DiffDays) >= 10)
            {
                // 星期一、二跨週，或是，星期三、四、五跨週又跨週（WeekDay + DiffDays >= 10），表示遇到的第一個交易日就是結算日，其他的就是星期三是結算日。
                weeklyOptionCandlesticks.Add(weeklyOptionCandlestick);

                weeklyOptionCandlestick = null;
            }
        }
    }
}

weeklyOptionCandlesticks.Dump();
return;

using (var writer = new StreamWriter(@"D:\Downloads\weekly-options.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(weeklyOptionCandlesticks);
}

public class Candlestick
{
    public DateTime Date { get; set; }

    [Name("Open")]
    public decimal Open { get; set; }

    [Name("High")]
    public decimal High { get; set; }

    [Name("Low")]
    public decimal Low { get; set; }

    [Name("Close")]
    public decimal Close { get; set; }

    public DayOfWeek WeekDay { get { return this.Date.DayOfWeek; } }
}