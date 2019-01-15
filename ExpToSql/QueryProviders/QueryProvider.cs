using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpToSql
{
    abstract class QueryProvider: IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            var targetType = typeof(Query<>).MakeGenericType(expression.Type);
            return (IQueryable)Activator.CreateInstance(targetType, this, expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        public abstract object Execute(Expression expression);

        public abstract TResult Execute<TResult>(Expression expression);
    }
}
