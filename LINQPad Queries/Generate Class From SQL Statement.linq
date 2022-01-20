<Query Kind="Program">
  <Connection>
    <ID>36051f11-dc98-402d-85c0-e1d32784a9f2</ID>
    <Persist>true</Persist>
    <Server>cartierloveisalltw.database.windows.net</Server>
    <SqlSecurity>true</SqlSecurity>
    <Database>cartierloveisalltw</Database>
    <UserName>sa_cartierloveisalltw</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAA9DvsGbqzSU2DYYUe13mP6AAAAAACAAAAAAAQZgAAAAEAACAAAAByIvw0WqyvmcsIjlBbtwHb9dDYbIxxY1bGed3KmjM3JgAAAAAOgAAAAAIAACAAAADwZdsM1WPP3C9Tkujy/U2MSNYdUx2XK/V7POTDgJf0XiAAAAAlHCcDgLnytTaDMrsijfilFY9n5zO31XfpaebBzdgcgkAAAADy08TCWl8Kpwnl1bDO10JrAsc871h8zd6rK1YgtWDwsTQYOuEuk1KvpaIgilOpSHmJg1X2NXJKhzyulL621pSz</Password>
    <DbVersion>Azure</DbVersion>
    <NoPluralization>true</NoPluralization>
  </Connection>
  <NuGetReference>Chef.Extensions.Agility</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
</Query>

void Main()
{
    this.Connection.DumpClass("SELECT TOP 1 * FROM MyTable WITH (NOLOCK)", "MyClass").Dump();
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