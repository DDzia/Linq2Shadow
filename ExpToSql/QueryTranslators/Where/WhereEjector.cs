using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpToSql.QueryTranslators
{
    class WhereEjector: ExpressionVisitor
    {
        public List<LambdaExpression> WhereExpressions = new List<LambdaExpression>();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable))
            {
                var prevQuery = node.Arguments[0];
                Visit(prevQuery);

                if (node.Method.Name == "Where")
                {
                    var lambda = (node.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
                    if (lambda == null)
                        throw new InvalidOperationException("Invalid where lambda");
                    WhereExpressions.Add(lambda);
                }

            }
            return node;
        }
    }
}
