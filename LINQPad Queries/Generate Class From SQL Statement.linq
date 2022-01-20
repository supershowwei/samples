<Query Kind="Program">
  <Connection>
    <ID>95e26ab1-da32-4de4-ad6e-85e06a82e1e2</ID>
    <Persist>true</Persist>
    <Server>elctaiwan.database.windows.net</Server>
    <SqlSecurity>true</SqlSecurity>
    <NoPluralization>true</NoPluralization>
    <UserName>sa_elctaiwan</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAA9DvsGbqzSU2DYYUe13mP6AAAAAACAAAAAAAQZgAAAAEAACAAAAAlCvdRILDIQMXbXAZvpvZoRMRwDnPIJvOZua9ZeNmD5wAAAAAOgAAAAAIAACAAAABp0bfmhhr5bKgaJKcR+G76yrTQNyNLgG+mOx3uV5IZzCAAAAChlzXCG7ubCQelQjB5ih/gPzaPRC93up1NeSCmTxGZm0AAAACLPVHhWvP0z2bBOSBm3F75vA/8BcyY4kd6f9r+sbW0v6u+abtslywkX/jqt+IjFS7/BiaVPWlG66EeE12T/RgV</Password>
    <DbVersion>Azure</DbVersion>
    <Database>elctaiwan</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
</Query>

void Main()
{
    var tableName = "SeniorityAward";
    var className = "SeniorityAward";

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