<Query Kind="Statements">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>CsvHelper.Configuration.Attributes</Namespace>
</Query>

var result = new List<object>();

using (var reader = new StreamReader(@"D:\Downloads\options\活頁簿2.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    var records = csv.GetRecords<DailyK>().ToList();

    //records.Dump();

    DailyK cross = null;

    foreach (var curt in records)
    {
        // 跨週
        if (curt.DiffWeekDays < 0)
        {
            cross = curt;
            cross.NextTradeDate = string.Empty;
            cross.NextOpen = decimal.MinValue;
            cross.NextHigh = decimal.MinValue;
            cross.NextLow = decimal.MaxValue;
            
            continue;
        }

        // 結算日
        if (cross != null)
        {
            if (cross.NextOpen < 0)
            {
                cross.NextOpen = curt.PrevOpen;
            }

            cross.NextTradeDate = curt.PrevTradeDate;
            cross.NextHigh = Math.Max(cross.NextHigh, curt.PrevHigh);
            cross.NextLow = Math.Min(cross.NextLow, curt.PrevLow);
            cross.NextClose = curt.PrevClose;
            
            if (cross.WeekDay < 3 || curt.WeekDay == 3 || (cross.WeekDay + cross.DiffDays) >= 10)
            {
                // 星期一、二跨週，或是，星期三、四、五跨週又跨週（WeekDay + DiffDays >= 10），表示遇到的第一個交易日就是結算日，其他的就是星期三是結算日。
                result.Add(cross);

                cross = null;
            }
        }
    }
}

result.Dump();

using (var writer = new StreamWriter(@"D:\Downloads\options\result.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    csv.WriteRecords(result);
}

public class DailyK
{
    [Name("TradeDate")]
    public string PrevTradeDate { get; set; }

    [Name("Open")]
    public decimal PrevOpen { get; set; }

    [Name("High")]
    public decimal PrevHigh { get; set; }

    [Name("Low")]
    public decimal PrevLow { get; set; }

    [Name("Close")]
    public decimal PrevClose { get; set; }

    public int WeekDay { get; set; }

    public int? DiffWeekDays { get; set; }

    public int? DiffDays { get; set; }

    [Name("TradeDate")]
    public string NextTradeDate { get; set; }

    [Name("Open")]
    public decimal NextOpen { get; set; }

    [Name("High")]
    public decimal NextHigh { get; set; }

    [Name("Low")]
    public decimal NextLow { get; set; }

    [Name("Close")]
    public decimal NextClose { get; set; }
}