using System;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2Shadow.Extensions
{
    /// <summary>
    /// Pack of extensions for IQueryable interface.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Select only fields.
        /// </summary>
        /// <typeparam name="T">The Type of the elements.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="fieldNames">Field names which will be included to destination query and destination object.</param>
        /// <returns>The constructed query to data source which presented via object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="query"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldNames"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="fieldNames"/> is empty array.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="fieldNames"/> contains null.</exception>
        public static IQueryable<T> SelectOnly<T>(this IQueryable<T> query, string[] fieldNames)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            if (fieldNames == null)
                throw new ArgumentNullException(nameof(fieldNames));

            if (!fieldNames.Any())
                throw new ArgumentException("Count of selected field must be more than one.", nameof(fieldNames));

            if(fieldNames.Any(x => x is null))
                throw new ArgumentException("Field name can't be null.", nameof(fieldNames));

            var miBase = typeof(QueryableExtensions).GetMethod(nameof(SelectOnly));
            var mi = miBase.MakeGenericMethod(typeof(T));

            var expr = Expression.Call(null, mi, query.Expression, Expression.Constant(fieldNames));

            return query.Provider.CreateQuery<T>(expr);
        }
    }
}
