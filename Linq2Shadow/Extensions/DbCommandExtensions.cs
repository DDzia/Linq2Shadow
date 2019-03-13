using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Linq2Shadow.Extensions
{
    internal static class DbCommandExtensions
    {
        public static List<ShadowRow> ReadAll(this IDbCommand cmd, CancellationToken cancellationToken = default)
        {
            var items = new List<ShadowRow>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read() && !cancellationToken.IsCancellationRequested)
                {
                    items.Add(reader.FillRow());
                }
            }
            return items;
        }

        public static async Task<List<ShadowRow>> ReadAllAsync(this DbCommand cmd, CancellationToken cancellationToken)
        {
            var items = new List<ShadowRow>();

            using (var reader = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (reader != null)
                {
                    while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false) && !cancellationToken.IsCancellationRequested)
                    {
                        var r = await reader.FillRow(cancellationToken).ConfigureAwait(false);
                        items.Add(r);
                    }
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return await CodeUtils.FromCancelledTask<List<ShadowRow>>(cancellationToken).ConfigureAwait(false);
            }

            return items;
        }

        private static async Task<ShadowRow> FillRow(this DbDataReader reader, CancellationToken cancellationToken = default(CancellationToken))
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

        private static ShadowRow FillRow(this IDataReader reader)
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
    }
}
