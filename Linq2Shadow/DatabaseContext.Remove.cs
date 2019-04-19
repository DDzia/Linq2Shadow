using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Linq2Shadow.QueryTranslators.Where;

namespace Linq2Shadow
{
    public partial class DatabaseContext
    {
        private static void ValidateRemoveArguments(string source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException(nameof(source));
        }

        /// <summary>
        /// Remove data from <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Data source like table or view.</param>
        /// <param name="condition">The remove condition. Record will be removed if she will be matched to this predicate, otherwise not.</param>
        /// <param name="cancellationToken">Operation cancellation.</param>
        /// <returns>Number of affected records.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> is whitespace.</exception>
        public async Task<int> RemoveAsync(string source,
            Expression<Func<ShadowRow, bool>> condition = null,
            CancellationToken cancellationToken = default)
        {
            ValidateRemoveArguments(source);

            using (var cmd = CreateRemoveCommand(source, condition))
            {
                return await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Remove data from <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Data source like table or view.</param>
        /// <param name="condition">The remove condition. Record will be removed if she will be matched to this predicate, otherwise not.</param>
        /// <returns>Number of affected records.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> is whitespace.</exception>
        public int Remove(string source, Expression<Func<ShadowRow, bool>> condition = null)
        {
            ValidateRemoveArguments(source);

            using (var cmd = CreateRemoveCommand(source, condition))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        private DbCommand CreateRemoveCommand(string removeFrom, Expression<Func<ShadowRow, bool>> predicate = null)
        {
            var sb = new StringBuilder()
                .Append("DELETE FROM ")
                .Append(removeFrom);

            var store = new QueryParametersStore();
            if (predicate != null)
            {
                var w = new WhereQueryTranslator(store, new[] { predicate });
                var whereSql = w.TranslateToSql(Expression.Empty());

                if (!string.IsNullOrWhiteSpace(whereSql))
                {
                    sb.Append(" WHERE ");
                    sb.Append(whereSql);
                }
            }

            var cmd = Connection.Value.CreateCommand();
            cmd.CommandText = sb.ToString();
            foreach (var param in store.GetParams())
            {
                var p = cmd.CreateParameter();
                p.ParameterName = param.Key;
                p.Value = param.Value;
                cmd.Parameters.Add(p);
            }

            return cmd;
        }
    }
}
