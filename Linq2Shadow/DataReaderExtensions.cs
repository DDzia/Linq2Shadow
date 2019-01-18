﻿using System.Collections.Generic;
using System.Data;

namespace Linq2Shadow
{
    internal static class DataReaderExtensions
    {
        public static IEnumerable<ShadowRow> ReadAll(this IDataReader reader)
        {
            var items = new List<ShadowRow>();

            while (reader.Read())
            {
                var row = new ShadowRow();
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
