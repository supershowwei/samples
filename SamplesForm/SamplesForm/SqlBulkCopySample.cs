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