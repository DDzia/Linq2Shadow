using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

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
                var value = reader.IsDBNull(i)
                    ? null
                    : reader[name];
                row.SetValue(name, value);
            }
            return row;
        }

        internal static async Task<ShadowRow> FillRowAsync(this DbDataReader reader, CancellationToken cancellationToken = default(CancellationToken))
        {
            var row = new ShadowRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = await reader.IsDBNullAsync(i, cancellationToken).ConfigureAwait(false)
                    ? null
                    : reader[name];
                row.SetValue(name, value);
            }
            return row;
        }
    }
}
