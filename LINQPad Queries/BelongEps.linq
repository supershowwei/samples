<Query Kind="Statements">
  <NuGetReference>Chef.DbAccess.SqlServer</NuGetReference>
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <Namespace>Chef.DbAccess</Namespace>
  <Namespace>Chef.DbAccess.SqlServer</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations</Namespace>
  <Namespace>System.ComponentModel.DataAnnotations.Schema</Namespace>
  <Namespace>Chef.Extensions.String</Namespace>
  <Namespace>Chef.DbAccess.Fluent</Namespace>
</Query>

// 開始允許季配息的日期
var date = new DateTime(2019, 1, 1);

SqlServerDataAccessFactory.Instance.AddConnectionString("TwStocks", "Server=dba.wantgoo.com;Database=twStocks;User Id=pma$-3a5B2347-7BF6-4506-8E26-7D0FFE1CA91D-$$;Password=$@@~~474FdB67-2AE6-42AF-8ADE-9925D43F0570-wantg00~~-$;");

IDataAccessFactory dataAccessFactory = SqlServerDataAccessFactory.Instance;

var stockDataAccess = dataAccessFactory.Create<Stock>();
var dividendPolicyDataAccess = dataAccessFactory.Create<DividendPolicy>();
var epsDataAccess = dataAccessFactory.Create<Eps>();

var aliveTime = DateTime.Today.AddMonths(-1);

var stocks = await stockDataAccess.Where(x => x.Time >= aliveTime && new[] { 0, 1, 4 }.Contains(x.Market))
                 .OrderBy(x => x.StockNo)
                 .Select(x => new { x.StockNo })
                 .QueryAsync();

// DEBUG
//stocks = stocks.Where(x => x.StockNo == "8049").ToList();

foreach (var stock in stocks)
{
    var dividendPolicies = await dividendPolicyDataAccess.Where(x => x.StockNo == stock.StockNo && x.CashDividend > 0)
                               .Select(x => new { x.StockNo, x.Date, x.Period, x.EPS })
                               .QueryAsync();

    if (dividendPolicies.Count == 0) continue;

    var epses = await epsDataAccess.Where(x => x.StockNo == stock.StockNo)
                    .Select(x => new { x.Year, x.Season, x.SEPS })
                    .QueryAsync();

    if (epses.Count == 0) continue;

    foreach (var dividendPolicy in dividendPolicies)
    {
        if (dividendPolicy.EPS.HasValue) continue;

        decimal eps;

        if (dividendPolicy.Date < date)
        {
            var yearEpses = epses.Where(x => x.Year == dividendPolicy.Date.Year - 1).ToList();

            if (yearEpses.Count == 0) continue;

            eps = yearEpses.Sum(x => x.SEPS);
        }
        else
        {
            if (string.IsNullOrEmpty(dividendPolicy.Period))
            {
                Console.WriteLine($"{stock.StockNo}_{dividendPolicy.Date}_NoPeriod");
                continue;
            }

            var seasons = ResolveSeasons(dividendPolicy);

            eps = SumEps(seasons, epses);
        }

        if (eps <= 0) continue;

        await dividendPolicyDataAccess.Where(x => x.StockNo == stock.StockNo && x.Date == dividendPolicy.Date)
            .Set(() => new DividendPolicy { EPS = eps })
            .UpdateAsync();

        Console.WriteLine($"{stock.StockNo}: {eps}");
    }
}

Console.WriteLine("Done.");

static List<(int, int)> ResolveSeasons(DividendPolicy dividend)
{
    return dividend.Period switch
    {
        var period when period.IsMatch(@"^(\d+)年度?$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 1, 2, 3, 4),
        var period when period.IsMatch(@"^(\d+)年第(\d)季$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, match.Groups[2].Value.ToInt32()),
        var period when period.IsMatch(@"^(\d+)年(?:上|前)半年度$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 1, 2),
        var period when period.IsMatch(@"^(\d+)年(?:下|後)半年度$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 3, 4),
        var period when period.IsMatch(@"^(\d+)年度(?:股票股利|現金股利)?及(\d+)年(?:上|前)半年度(?:股票股利|現金股利)?$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 1, 2, 3, 4).Concat(CreateSeasons(match.Groups[2].Value.ToInt32() + 1911, 1, 2)).ToList(),
        var period when period.IsMatch(@"^(\d+)年度(?:股票股利|現金股利)?及(\d+)年(?:下|後)半年度(?:股票股利|現金股利)?$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 1, 2, 3, 4).Concat(CreateSeasons(match.Groups[2].Value.ToInt32() + 1911, 3, 4)).ToList(),
        var period when period.IsMatch(@"^(\d+)年度及(\d+)年第(\d)季$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 1, 2, 3, 4).Concat(CreateSeasons(match.Groups[2].Value.ToInt32() + 1911, match.Groups[3].Value.ToInt32())).ToList(),
        var period when period.IsMatch(@"^(\d+)年度前(\d)季$", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, Enumerable.Range(1, match.Groups[2].Value.ToInt32()).ToArray()),
        var period when period.IsMatch(@"^(\d+)年度及(\d+)年前(\d)季", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 1, 2, 3, 4).Concat(CreateSeasons(match.Groups[2].Value.ToInt32() + 1911, Enumerable.Range(1, match.Groups[3].Value.ToInt32()).ToArray())).ToList(),
        var period when period.IsMatch(@"^(\d+)年及(\d+)年度", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 1, 2, 3, 4).Concat(CreateSeasons(match.Groups[2].Value.ToInt32() + 1911, 1, 2, 3, 4)).ToList(),
        var period when period.IsMatch(@"^(\d+)年第(\d+)季及(\d+)年第(\d+)季", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, match.Groups[2].Value.ToInt32()).Concat(CreateSeasons(match.Groups[3].Value.ToInt32() + 1911, match.Groups[4].Value.ToInt32())).ToList(),
        var period when period.IsMatch(@"^(?:\d+年度、)?(\d+)年第(\d+)季及(\d+)年第(\d+)季", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, match.Groups[2].Value.ToInt32()).Concat(CreateSeasons(match.Groups[3].Value.ToInt32() + 1911, match.Groups[4].Value.ToInt32())).ToList(),
        var period when period.IsMatch(@"^(\d+)後半年度盈餘分派暨\d+年度資本公積發放現金", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 3, 4).ToList(),
        var period when period.IsMatch(@"^(\d+)年後半年度及(\d+)年前半年度", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, 3, 4).Concat(CreateSeasons(match.Groups[2].Value.ToInt32() + 1911, 1, 2)).ToList(),
        var period when period.IsMatch(@"^(\d+)年度第(\d+)季及第(\d+)季", out var match) => CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, match.Groups[2].Value.ToInt32()).Concat(CreateSeasons(match.Groups[1].Value.ToInt32() + 1911, match.Groups[3].Value.ToInt32())).ToList(),
        _ => null
    };
}

static decimal SumEps(List<(int, int)> seasons, List<Eps> epses)
{
    return seasons == null
               ? 0m
               : epses.Where(
                       e => seasons.Any(
                           s =>
                           {
                               var (year, season) = s;

                               return e.Year == year && e.Season == season;
                           }))
                   .Sum(e => e.SEPS);
}

static List<(int, int)> CreateSeasons(int year, params int[] seasons)
{
    return seasons.Select(s => (year, s)).ToList();
}

[ConnectionString("TwStocks")]
[Table("Exright")]
public class DividendPolicy
{
    public string StockNo { get; set; }

    public DateTime Date { get; set; }

    public decimal CashDividend { get; set; }

    public string Period { get; set; }

    public decimal? EPS { get; set; }
}

[ConnectionString("TwStocks")]
[Table("GPMMerge_Isolation")]
public class Eps
{
    public string StockNo { get; set; }

    public int Year { get; set; }

    public int Season { get; set; }

    public decimal SEPS { get; set; }
}

[ConnectionString("TwStocks")]
public class Stock
{
    [Column(TypeName = "nvarchar")]
    [StringLength(10)]
    public string StockNo { get; set; }

    public DateTime Time { get; set; }

    public int Market { get; set; }
}