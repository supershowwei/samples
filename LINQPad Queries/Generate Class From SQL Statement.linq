<Query Kind="Program">
  <Connection>
    <ID>3c15d999-9d27-45e4-866f-0443f5cae520</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Server>dba.wantgoo.com</Server>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <SqlSecurity>true</SqlSecurity>
    <UserName>pma$-3a5B2347-7BF6-4506-8E26-7D0FFE1CA91D-$$</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAcJk1ehO/b0OnprrtF8EikwAAAAACAAAAAAAQZgAAAAEAACAAAABZ5w2YZO1FMOrrmngfQVjCiZL5Ibsr4+nhHsNJfurH2wAAAAAOgAAAAAIAACAAAACrVskH927EYZeL+l0lFOAUsxOPEQqaOsmwzC+VzvnT7EAAAABQiyOftzpKK1J9+joLVUzIL5JLzWdLeL7XD6+z57vi6AlbUGe3LkOcsWXgvUNXEQIkCoUHm64Bpl0S/V2ePHyuQAAAAOG3w+G6F862594O4MlGLL7MlXPuzgVEHDqXRu9lqWuao66pZ0J02h2lwP1JPiLWqd0ZdlJIU5lmWncqyRvv98g=</Password>
    <Database>twStocks</Database>
    <DriverData>
      <LegacyMFA>false</LegacyMFA>
    </DriverData>
  </Connection>
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
</Query>

void Main()
{
    var tableName = "ETFIncomeSource";
    var className = "ETFIncomeSource";

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