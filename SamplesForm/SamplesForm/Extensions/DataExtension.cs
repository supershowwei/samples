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
            return
                typeof(T).GetProperties()
                    .ToDictionary(
                        property =>
                            new DataColumn(
                                GetPropertyName(property),
                                Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType));
        }

        private static string GetPropertyName(PropertyInfo property)
        {
            var columnAttr = property.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(ColumnAttribute));

            var propertyName = columnAttr != null
                                   ? columnAttr.NamedArguments.Single(x => x.MemberName.Equals("Name")).TypedValue.Value
                                   : property.Name;

            return (string)propertyName;
        }
    }
}