using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using ArchitectSample.Protocol.Model.Data;
using Chef.Extensions.DbAccess;
using Microsoft.Extensions.Configuration;

namespace ArchitectSample.Physical.DataAccesses
{
    public class ClubDataAccess : SqlServerDataAccess<Club>
    {
        public ClubDataAccess(IConfiguration configuration)
            : base(configuration.GetConnectionString("MSSQLLocalDB"))
        {
        }

        protected override Expression<Func<Club, object>> DefaultSelector { get; } = x => new { x.Id, x.Name };

        protected override Expression<Func<Club>> RequiredColumns { get; } =
            () => new Club { Id = default, Name = default, IsActive = default };

        protected override (string, DataTable) ConvertToTableValuedParameters(IEnumerable<Club> values)
        {
            var dataTable = CreateDataTable();

            foreach (var value in values)
            {
                dataTable.Rows.Add(CreateDataRow(dataTable, value));
            }

            return ("ClubType", dataTable);
        }

        private static DataTable CreateDataTable()
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("ClubID", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("IsActive", typeof(bool));

            return dataTable;
        }

        private static DataRow CreateDataRow(DataTable dataTable, Club value)
        {
            var dataRow = dataTable.NewRow();

            dataRow["ClubID"] = value.Id;
            dataRow["Name"] = value.Name;
            dataRow["IsActive"] = value.IsActive;

            return dataRow;
        }
    }
}