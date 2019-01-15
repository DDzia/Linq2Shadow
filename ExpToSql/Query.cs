using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpToSql
{
    class Query<T> : IOrderedQueryable<T>
    {
        public Type ElementType => typeof(T);
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        //internal Query()
        //{
        //    Provider = new SqlQueryProvider();
        //    Expression = Expression.Constant(this);
        //}

        internal Query(IQueryProvider provider, Expression expr = null)
        {
            Provider = provider;
            Expression = expr ?? Expression.Constant(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
