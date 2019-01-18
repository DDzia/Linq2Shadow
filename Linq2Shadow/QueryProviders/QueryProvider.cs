using System;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2Shadow.QueryProviders
{
    internal abstract class QueryProvider: IQueryProvider
    {
        protected readonly QueryParametersStore _queryParamsStore;

        protected internal QueryProvider(QueryParametersStore queryParamsStore = null)
        {
            _queryParamsStore = queryParamsStore ?? new QueryParametersStore();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var targetType = typeof(Query<>).MakeGenericType(expression.Type);
            return (IQueryable)Activator.CreateInstance(targetType, this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotSupportedException();
        }

        public abstract TResult Execute<TResult>(Expression expression);
    }
}
