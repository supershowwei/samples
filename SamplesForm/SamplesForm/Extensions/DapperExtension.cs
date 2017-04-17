using System;
using System.Collections.Generic;
using System.Data;
using Dapper;

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

        private static Dictionary<string, Func<IDataReader, T>> GenerateParsers<T>(IDataReader reader)
        {
            //if (typeof(T) == typeof(OrderProduct))
            //{
            //    return new Dictionary<string, Func<IDataReader, T>>
            //               {
            //                   [ProductType.Rail.ToString()] =
            //                   reader.GetRowParser<T>(typeof(OrderRail))
            //               };
            //}

            throw new ArgumentOutOfRangeException();
        }
    }
}