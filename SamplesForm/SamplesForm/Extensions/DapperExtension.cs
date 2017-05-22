using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using Dapper;
using SamplesForm.Model.Parameters;

namespace SamplesForm.Extensions
{
    public static class DapperExtension
    {
        public static IEnumerable<T> PolymorphicQuery<T>(
            this IDbConnection cnn,
            string sql,
            string discriminator,
            object param = null)
        {
            var result = new List<T>();

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                var parsers = GenerateParsers<T>(reader);

                var typeColumnIndex = reader.GetOrdinal(discriminator);

                while (reader.Read())
                {
                    result.Add(parsers[reader.GetString(typeColumnIndex)](reader));
                }
            }

            return result;
        }

        public static string GenerateUpdatingSql(
            this object me,
            string table,
            DynamicParameters parameters,
            string suffix = "")
        {
            var udpatedFields =
                me.GetType()
                    .GetProperties()
                    .Where(
                        p =>
                            p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Field<>)
                            && p.GetValue(me) != null)
                    .Select(x => new { Name = GetFieldName(x), Property = x })
                    .ToList();

            var sets =
                udpatedFields.Select(f => string.Format("{0} = @{1}{2}", f.Name, f.Property.Name, suffix)).ToList();

            udpatedFields.ForEach(
                field =>
                    {
                        var name = string.Concat(field.Property.Name, suffix);
                        var value = field.Property.GetValue(me);

                        parameters.Add(name, value.GetType().GetProperty("Value").GetValue(value));
                    });

            var clauseFields =
                me.GetType()
                    .GetProperties()
                    .Where(p => !p.PropertyType.IsGenericType)
                    .Select(x => new { Name = GetFieldName(x), Property = x })
                    .ToList();

            var clauses =
                clauseFields.Select(f => string.Format("{0} = @{1}{2}", f.Name, f.Property.Name, suffix)).ToList();

            clauseFields.ForEach(
                field =>
                    {
                        var fieldName = string.Concat(field.Property.Name, suffix);
                        var fieldValue = field.Property.GetValue(me);

                        parameters.Add(fieldName, fieldValue);
                    });

            return @"
UPDATE " + table + @"
SET " + string.Join(", ", sets) + @"
WHERE " + string.Join(" AND ", clauses);
        }

        private static Dictionary<string, Func<IDataReader, T>> GenerateParsers<T>(IDataReader reader)
        {
            // if (typeof(T) == typeof(OrderProduct))
            // {
            // return new Dictionary<string, Func<IDataReader, T>>
            // {
            // [ProductType.Rail.ToString()] =
            // reader.GetRowParser<T>(typeof(OrderRail))
            // };
            // }
            throw new ArgumentOutOfRangeException();
        }

        private static string GetFieldName(PropertyInfo x)
        {
            var columnAttr = x.CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(ColumnAttribute));

            return columnAttr != null ? (string)columnAttr.ConstructorArguments[0].Value : x.Name;
        }
    }
}