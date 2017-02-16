using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace SamplesForm
{
    public class DynamicParametersSample
    {
        /// <summary>
        ///     用 DynamicParameters 兜多個 LIKE 條件
        /// </summary>
        public void Sample1()
        {
            var startParts = new List<string> { "WTX", "WTF" };
            var clauses = new List<string>();
            var parameters = new DynamicParameters();

            for (var index = 0; index < startParts.Count; index++)
            {
                clauses.Add("s.StockNo LIKE @StartsWith_" + index);
                parameters.Add("StartsWith_" + index, startParts[index] + "%");
            }

            parameters.Add("BeginningInsertedTime", new DateTime());
            parameters.Add("EndInsertedTime", new DateTime());
            parameters.Add("GreaterOrEqualTime", new DateTime());

            var sql = @"
SELECT
    s.StockNo AS Code
   ,rtp.[Time]
   ,rtp.Deal
   ,rtp.Change
   ,rtp.Volume
   ,rtp.TotalVolume
   ,rtp.InsertTime
FROM RealTimePrice rtp WITH (NOLOCK)
LEFT JOIN [LINKDBSERVER].twFutures.dbo.Stock s WITH (NOLOCK)
    ON rtp.StockNo = s.StockNo
WHERE rtp.InsertTime >= @BeginningInsertedTime
AND rtp.InsertTime < @EndInsertedTime
AND rtp.[Time] >= @GreaterOrEqualTime
AND s.StockNo NOT LIKE '%;%'
AND (" + (clauses.Any() ? string.Join(" OR ", clauses) : "1 = 1") + @")";

            using (var db = new SqlConnection(string.Empty))
            {
                var result = db.Query(sql, parameters).ToList();
            }
        }
    }
}