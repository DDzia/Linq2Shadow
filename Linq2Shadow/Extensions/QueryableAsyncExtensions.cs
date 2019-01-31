using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Linq2Shadow.Extensions
{
    public static class QueryableAsyncExtensions
    {
        /// <summary>
        /// Get List of data asynchronously.
        /// </summary>
        /// <typeparam name="T">The Type of the elements.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task which should return List of data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <param name="query"></param> is null.</exception>
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            var miBase = typeof(QueryableAsyncExtensions).GetMethod(nameof(ToListAsync));
            var mi = miBase.MakeGenericMethod(typeof(T));

            var expr = Expression.Call(null, mi, query.Expression, Expression.Constant(cancellationToken));

            return query.Provider.Execute<Task<List<T>>>(expr);
        }
    }
}
