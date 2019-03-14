using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Linq2Shadow.QueryProviders;

namespace Linq2Shadow
{
    class Query<T> : IOrderedQueryable<T>
    {
        public Type ElementType => typeof(T);
        public Expression Expression { get; }

        private readonly QueryProvider _qProvider;
        public IQueryProvider Provider => _qProvider;

        internal Query(QueryProvider provider, Expression expr = null)
        {
            _qProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            Expression = expr ?? Expression.Constant(this);
        }

        public IEnumerator<T> GetEnumerator() => _qProvider.GetEnumerator<T>(Expression);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
