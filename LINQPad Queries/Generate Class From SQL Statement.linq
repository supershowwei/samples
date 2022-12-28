<Query Kind="Program">
  <Connection>
    <ID>e4e65ae3-89eb-44b9-a0d0-b63a313afc5c</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>db.wantgoo.local</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <SqlSecurity>true</SqlSecurity>
    <UserName>pma$-3a5B2347-7BF6-4506-8E26-7D0FFE1CA91D-$$</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAcJk1ehO/b0OnprrtF8EikwAAAAACAAAAAAAQZgAAAAEAACAAAAAUu4aRixkr1655yHtegRA16W/dzvAeLIcuHRwSCdkTZQAAAAAOgAAAAAIAACAAAABytvrTGSJfP3csoEBGxwxMZ1a72LX3cvYcRHcrz+efI0AAAACcgWx/ua7v7k4LTiU9/O2VxiDoAaObDt9evEjX/ZKdtXAoHAhe3JJD2rSkvitQJRSFFfVpvx8aRdxqJ+W5W+hhQAAAAPXccA9AM0fqBAP6XDdDofmoFg8wYp6vQFDPIS1WT0LXctaRkQM3lY4+uckobEvRU0nAP0EacUpCh2dPe3hRSpY=</Password>
    <Database>Club</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
</Query>

void Main()
{
    var tableName = "ClubRemuneration";
    var className = "ClubRemuneration";

    this.Connection.DumpClass($"SELECT TOP 1 * FROM {tableName} WITH (NOLOCK)", className).Dump();
}

// Define other methods and classes here
public static class LINQPadExtensions
{
    private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string> {
        { typeof(int), "int" },
        { typeof(short), "short" },
        { typeof(byte), "byte" },
        { typeof(byte[]), "byte[]" },
        { typeof(long), "long" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(float), "float" },
        { typeof(bool), "bool" },
        { typeof(string), "string" }
    };

    private static readonly HashSet<Type> NullableTypes = new HashSet<Type> {
        typeof(int),
        typeof(short),
        typeof(long),
        typeof(double),
        typeof(decimal),
        typeof(float),
        typeof(bool),
        typeof(DateTime)
    };

    public static string DumpClass(this IDbConnection connection, string sql, string className = "Info")
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        var reader = cmd.ExecuteReader();

        var builder = new StringBuilder();
        do
        {
            if (reader.FieldCount <= 1) continue;

            builder.AppendFormat("public class {0}{1}", className, Environment.NewLine);
            builder.AppendLine("{");
            var schema = reader.GetSchemaTable();

            foreach (DataRow row in schema.Rows)
            {
                var type = (Type)row["DataType"];
                var name = TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.Name;
                var isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(type);
                var collumnName = (string)row["ColumnName"];

                builder.AppendLine(string.Format("\tpublic {0}{1} {2} {{ get; set; }}", name, isNullable ? "?" : string.Empty, collumnName));
                builder.AppendLine();
            }

            builder.AppendLine("}");
            builder.AppendLine();
        } while (reader.NextResult());

        return builder.ToString();
    }
}