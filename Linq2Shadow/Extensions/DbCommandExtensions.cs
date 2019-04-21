using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Linq2Shadow.Extensions
{
    internal static class DbCommandExtensions
    {
        internal static List<ShadowRow> ReadAll(this DbCommand cmd, CancellationToken cancellationToken = default)
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
                        items.Add(reader.FillRow());
                    }
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return await CodeUtils.FromCancelledTask<List<ShadowRow>>(cancellationToken).ConfigureAwait(false);
            }

            return items;
        }
    }
}
