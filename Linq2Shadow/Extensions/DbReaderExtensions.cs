using System;
using System.Data.Common;

namespace Linq2Shadow.Extensions
{
    internal static class DbReaderExtensions
    {
        internal static ShadowRow FillRow(this DbDataReader reader)
        {
            var row = new ShadowRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader[name];
                value = value == DBNull.Value
                    ? null
                    : value;

                row.SetValue(name, value);
            }
            return row;
        }
    }
}
