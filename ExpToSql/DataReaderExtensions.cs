using System.Collections.Generic;
using System.Data;

namespace ExpToSql
{
    internal static class DataReaderExtensions
    {
        public static IEnumerable<DynamicRow> ReadAll(this IDataReader reader)
        {
            var items = new List<DynamicRow>();

            while (reader.Read())
            {
                var row = new DynamicRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var value = reader.GetValue(i);
                    row.SetValue(name, value);
                }
                items.Add(row);
            }

            return items;
        }
    }
}
