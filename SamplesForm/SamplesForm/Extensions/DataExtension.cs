using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace SamplesForm.Extensions
{
    public static class DataExtension
    {
        public static DataTable ToDataTable<T>(this List<T> me)
        {
            var table = new DataTable();

            var properties = GeneratePropertiesOfCorrespondingColumn<T>();

            table.Columns.AddRange(properties.Keys.ToArray());

            me.ForEach(
                item =>
                    {
                        var row = table.NewRow();

                        foreach (var property in properties)
                        {
                            row[property.Key] = property.Value.GetValue(item) ?? DBNull.Value;
                        }

                        table.Rows.Add(row);
                    });

            return table;
        }

        private static Dictionary<DataColumn, PropertyInfo> GeneratePropertiesOfCorrespondingColumn<T>()
        {
            var orderedProperties = typeof(T).GetProperties().Where(p => p.GetMethod.IsPublic).Select(
                p =>
                    {
                        var columnAttr =
                            p.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(ColumnAttribute));

                        var namedArguments = columnAttr != null
                                                 ? columnAttr.NamedArguments.ToDictionary(
                                                     x => x.MemberName,
                                                     x => x.TypedValue.Value)
                                                 : null;

                        return
                            new
                                {
                                    Order =
                                    (namedArguments != null) && namedArguments.ContainsKey("Order")
                                        ? (int)namedArguments["Order"]
                                        : int.MaxValue,
                                    Name =
                                    (namedArguments != null) && namedArguments.ContainsKey("Name")
                                        ? (string)namedArguments["Name"]
                                        : p.Name,
                                    Value = p
                                };
                    }).OrderBy(x => x.Order);

            return
                orderedProperties.ToDictionary(
                    property =>
                        new DataColumn(
                            property.Name,
                            Nullable.GetUnderlyingType(property.Value.PropertyType) ?? property.Value.PropertyType),
                    property => property.Value);
        }
    }
}