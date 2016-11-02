using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace SamplesForm.Utilities
{
    internal static class SqlBulkCopyUtility
    {
        /// <summary>
        ///     Using System.Data.Linq.Mapping.ColumnAttribute define specific column name.
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public static Dictionary<DataColumn, PropertyInfo> GetPropertiesOfCorrespondingColumn<T>(
            DataColumnCollection columns)
        {
            var properties = new Dictionary<DataColumn, PropertyInfo>();

            foreach (DataColumn column in columns)
            {
                var property = typeof(T).GetProperties().SingleOrDefault(
                    p =>
                    {
                        var columnAttribute =
                            p.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(ColumnAttribute));

                        var propertyName = columnAttribute != null
                                               ? (string)
                                               columnAttribute.NamedArguments.Single(
                                                   x => x.MemberName.Equals("Name")).TypedValue.Value
                                               : p.Name;

                        return column.ColumnName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase);
                    });

                properties.Add(column, property);
            }

            return properties;
        }

        public static void InsertDataRows<T>(
            List<T> objects,
            Dictionary<DataColumn, PropertyInfo> properties,
            DataTable table)
        {
            objects.ForEach(
                trans =>
                {
                    var row = table.NewRow();

                    foreach (var property in properties)
                    {
                        row[property.Key] = property.Value == null ? null : property.Value.GetValue(trans);
                    }

                    table.Rows.Add(row);
                });
        }
    }
}