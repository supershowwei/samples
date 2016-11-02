using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using Dapper;
using SamplesForm.Model.Data;
using SamplesForm.Utilities;

namespace SamplesForm
{
    public class SqlBulkCopySample
    {
        private readonly string twstocksOfWantgooDbStockConnectionString =
            ConfigurationManager.ConnectionStrings["wantgoodbstock.twstocks"].ConnectionString;

        public void BulkInsert(List<SecuritiesTransaction> transactions)
        {
            var table = CreateSchema();

            var properties = SqlBulkCopyUtility.GetPropertiesOfCorrespondingColumn<SecuritiesTransaction>(table.Columns);

            SqlBulkCopyUtility.InsertDataRows(transactions, properties, table);

            using (var tx = new TransactionScope())
            {
                using (var sql = new SqlConnection(this.twstocksOfWantgooDbStockConnectionString))
                {
                    sql.Open();

                    var sb = new StringBuilder();
                    sb.AppendLine(@"DELETE FROM [dbo].[StockAgentTransactionData_TestData]");
                    sb.AppendLine(@"WHERE [StockNo] = @StockNo");
                    sb.AppendLine(@"      AND [Date] = @Date;");

                    // 刪除舊有資料
                    sql.Execute(sb.ToString(), new { transactions.First().StockNo, transactions.First().Date });

                    // 批次新增
                    using (var bulkCopy = new SqlBulkCopy(sql))
                    {
                        bulkCopy.DestinationTableName = "dbo.StockAgentTransactionData_TestData";
                        bulkCopy.WriteToServer(table);
                    }
                }

                tx.Complete();
            }

            ////設定一個批次量寫入多少筆資料
            //sqlBC.BatchSize = 1000;

            ////設定逾時的秒數
            //sqlBC.BulkCopyTimeout = 60;

            ////設定 NotifyAfter 屬性，以便在每複製 10000 個資料列至資料表後，呼叫事件處理常式。
            //sqlBC.NotifyAfter = 10000;
            //sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

            ////設定要寫入的資料庫
            //sqlBC.DestinationTableName = "dbo.Table1";

            ////對應資料行
            //sqlBC.ColumnMappings.Add("id", "id");
            //sqlBC.ColumnMappings.Add("name", "name");

            ////開始寫入
            //sqlBC.WriteToServer(dt);
        }

        private static DataTable CreateSchema()
        {
            var dt = new DataTable();
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("StockAgentIdx", typeof(string));
            dt.Columns.Add("StockNo", typeof(string));
            dt.Columns.Add("Price", typeof(double));
            dt.Columns.Add("BuyCount", typeof(double));
            dt.Columns.Add("SellCount", typeof(double));

            return dt;
        }
    }
}