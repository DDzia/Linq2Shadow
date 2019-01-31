using System;
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

        public static Task<List<ShadowRow>> ReadAllAsync(this IDbCommand cmd, CancellationToken cancellationToken)
        {
            if (cmd is DbCommand dbCmd)
            {
                return ReadAllAsync(dbCmd, cancellationToken);
            }

            return ReadAllViaThreadPoolWorkItemAsync(cmd, cancellationToken);
        }

        private static async Task<List<ShadowRow>> ReadAllAsync(this DbCommand cmd, CancellationToken cancellationToken)
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
                return await Task.FromCanceled<List<ShadowRow>>(cancellationToken).ConfigureAwait(false);
            }

            return items;
        }

        private static Task<List<ShadowRow>> ReadAllViaThreadPoolWorkItemAsync(this IDbCommand cmd, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<List<ShadowRow>>();

            Task.Run(() =>
            {
                try
                {
                    var items = ReadAll(cmd, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.SetCanceled();
                    }

                    tcs.SetResult(items);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, cancellationToken);

            return tcs.Task;
        }

        private static ShadowRow FillRow(this IDataReader reader)
        {
            var row = new ShadowRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var value = reader.IsDBNull(i)
                    ? null
                    : reader.GetValue(i);
                row.SetValue(name, value);
            }
            return row;
        }
    }
}
