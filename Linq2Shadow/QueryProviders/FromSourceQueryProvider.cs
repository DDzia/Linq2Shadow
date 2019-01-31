using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Linq2Shadow.Extensions;
using Linq2Shadow.QueryTranslators;

namespace Linq2Shadow.QueryProviders
{
    /// <summary>
    /// The LINQ query provider for building queries to database sources.
    /// </summary>
    /// <remarks>
    /// Source can be any.
    /// For example: for source we can use table-valued function call or view name or table name.
    /// </remarks>
    /// <seealso cref="QueryProvider" />
    internal class FromSourceQueryProvider: QueryProvider
    {
        private readonly string _source;

        private readonly DatabaseContext _dbCtx;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromSourceQueryProvider"/> class.
        /// </summary>
        /// <param name="dbCtx">The database context.</param>
        /// <param name="source">The query source.</param>
        /// <param name="queryParamsStore">The store of query parameters.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw if source or database context is NULL.
        /// </exception>
        /// <exception cref="System.ArgumentException">Throw when source has invalid value.</exception>
        internal FromSourceQueryProvider(DatabaseContext dbCtx,
                string source,
                QueryParametersStore queryParamsStore = null)
            :base(queryParamsStore)
        {
            _dbCtx = dbCtx ?? throw new ArgumentNullException(nameof(dbCtx));

            source = source ?? throw new ArgumentNullException(nameof(source));
            _source = string.IsNullOrWhiteSpace(source) ? throw new ArgumentException(nameof(source)) : source;
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public override TResult Execute<TResult>(Expression expression)
        {
            // generate SQL from expression trees
            var sqlGenerated = new FromSourceQueryTranslator(_queryParamsStore, _source)
                .TranslateToSql(expression);

            IDbCommand PrepareSqlAndParams()
            {
                var cmd = _dbCtx.Connection.Value.CreateCommand();

                cmd.CommandText = sqlGenerated;

                foreach (var paramKv in _queryParamsStore.GetParams())
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = paramKv.Key;
                    p.Value = paramKv.Value;
                    cmd.Parameters.Add(p);
                }

                return cmd;
            }

            // it is a Count aggregation call
            if (ExpressionsInternalToolkit.IsCountQueryableCall(expression))
            {
                using (var cmd = PrepareSqlAndParams())
                {
                    var val = cmd.ExecuteScalar();

                    var skipCount = ExpressionsInternalToolkit.GetSkipCount(expression);
                    var countViaSkipAdjusted = Math.Max((int) val - skipCount, 0);
                    if (ExpressionsInternalToolkit.TakeIsUsed(expression))
                    {
                        var takeCount = ExpressionsInternalToolkit.GetTakeCount(expression);
                        var countViaTakeAdjusted = Math.Min(countViaSkipAdjusted, takeCount);
                        return (TResult) (object) countViaTakeAdjusted;
                    }

                    return (TResult) (object) Math.Max((int) val - skipCount, 0);
                }
            }

            // otherwise is applied a Query
            using (var cmd = PrepareSqlAndParams())
            {
                if (ExpressionsInternalToolkit.IsListAsyncCall(expression))
                {
                    var ct = ExpressionsInternalToolkit.GetCancellationTokenForToList(expression);
                    return (TResult)(object)cmd.ReadAllAsync(ct);
                }
                else
                {
                    var collection = cmd.ReadAll();

                    var firstAggregationCalled = ExpressionsInternalToolkit.IsFirstQueryableCall(expression);
                    var firstOrDefaultAggregationCalled = ExpressionsInternalToolkit.IsFirstOrDefaultQueryableCall(expression);
                    if (firstAggregationCalled || firstOrDefaultAggregationCalled)
                    {
                        if (collection.Count > 1)
                        {
                            throw new InvalidOperationException("Unexpected collection length.");
                        }

                        if (firstAggregationCalled)
                        {
                            var first = collection.FirstOrDefault();
                            if (first == null)
                            {
                                throw new InvalidOperationException("Data not found.");
                            }
                            return (TResult)(object)first;
                        }

                        if (firstOrDefaultAggregationCalled)
                        {
                            return (TResult)(object)collection.FirstOrDefault();
                        }
                    }

                    return (TResult)(object)collection;
                }
            }
        }
    }
}
