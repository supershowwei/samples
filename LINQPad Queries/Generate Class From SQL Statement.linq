<Query Kind="Program">
  <Connection>
    <ID>0bcf1927-263f-46ae-b44e-27919fa73756</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>35.187.152.118</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <SqlSecurity>true</SqlSecurity>
    <UserName>pma$-3a5B2347-7BF6-4506-8E26-7D0FFE1CA91D-$$</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAA8k6fSZv6Fkaf9CefTm/ykgAAAAACAAAAAAAQZgAAAAEAACAAAACsQ635bEcXtmURTRaHfKGmmeqYr6vk3sQ+A5iXuw5vGQAAAAAOgAAAAAIAACAAAABl5UfDph7y/AWvDwJiKA4+WLQCxOjNkAoMW4l1hCpQHkAAAADw/gQv9jgkbFnUpmNcll7fMJPSVic0RSelvPWkZCrKyJNTPT8H3vyVn/dX6Qdabm0nU78U4w/S/yfq3jfb0f+oQAAAACjD25xXnYBZzOjW9i+/0mtuhKAEe6itMCqzrzCVxXEQY8aLvXM1ROxGmCJ0BiyOMk+tOpz34U/tbiGxAOl2wFI=</Password>
    <Database>Fintune</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
</Query>

void Main()
{
    var tableName = "RetroTechnicalIndicator";
    var className = "RetroTechnicalIndicator";

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
            builder.Append("{");
            var schema = reader.GetSchemaTable();

            foreach (DataRow row in schema.Rows)
            {
                var type = (Type)row["DataType"];
                var name = TypeAliases.ContainsKey(type) ? TypeAliases[type] : type.Name;
                var isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(type);
                var collumnName = (string)row["ColumnName"];

                builder.AppendLine();
                builder.AppendLine(string.Format("\tpublic {0}{1} {2} {{ get; set; }}", name, isNullable ? "?" : string.Empty, collumnName));
            }

            builder.AppendLine("}");
            builder.AppendLine();
        } while (reader.NextResult());

        return builder.ToString();
    }
}