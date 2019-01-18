using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2Shadow.QueryTranslators.OrderBy
{
    internal class OrderByEjector: ExpressionVisitor
    {
        public List<Tuple<LambdaExpression, bool>> OrderByExpressions = new List<Tuple<LambdaExpression, bool>>();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                foreach (var prevQuery in node.Arguments)
                {
                    Visit(prevQuery);
                }

                if (node.Method.Name == nameof(Queryable.OrderBy)
                    || node.Method.Name == nameof(Queryable.OrderByDescending))
                {
                    // create prev ordering if new was been setted by OrderBy or OrderByDescending calls
                    OrderByExpressions.Clear();
                }

                
                if (node.Method.Name == nameof(Queryable.OrderBy) ||
                    node.Method.Name == nameof(Queryable.OrderByDescending) ||
                    node.Method.Name == nameof(Queryable.ThenBy) ||
                    node.Method.Name == nameof(Queryable.ThenByDescending))
                {
                    var lambda = (node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                    if (lambda == null)
                    {
                        throw new InvalidOperationException("Invalid where lambda");
                    }

                    var asc = node.Method.Name == nameof(Queryable.OrderBy) ||
                        node.Method.Name == nameof(Queryable.ThenBy);

                    OrderByExpressions.Add(new Tuple<LambdaExpression, bool>(lambda, asc));
                }
            }
            return node;
        }
    }
}
