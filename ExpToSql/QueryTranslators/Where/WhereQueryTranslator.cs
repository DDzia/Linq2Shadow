using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpToSql.Visitors;

namespace ExpToSql.QueryTranslators.Where
{
    class WhereQueryTranslator: QueryTranslator
    {
        private readonly WhereEjector whereEjector = new WhereEjector();
        private bool whereAdded = false;

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Visit(node.Body);

            return base.VisitLambda(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Visit(node.Object);

            if (node.Object is ParameterExpression parmtr &&
                node.Method.Name == "get_Item"
                && node.Method.DeclaringType == typeof(DynamicRow))
            {
                var arg = node.Arguments[0] as ConstantExpression;
                if(arg is null)
                    throw new Exception();

                _sb.Append(arg.Value);
                

            }

            if (node.Method.Name == nameof(Boolean.Equals))
            {
                Visit(node.Object);

                var valueExpr = node.Arguments[0];

                var cValueExpr = valueExpr is ConstantExpression
                    ? valueExpr as ConstantExpression
                    : (valueExpr as UnaryExpression).Operand as ConstantExpression;

                var value = cValueExpr.Value;
                if(value)

                if (valueExpr is UnaryExpression uExpr)
                {
                    (uExpr.Operand as ConstantExpression).Value;
                }
                   


            }

            return node;
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        public override string Translate2Sql(Expression expr)
        {
            whereEjector.Visit(expr);
            foreach (var whereExpression in whereEjector.WhereExpressions)
            {
                Visit(whereExpression);
            }

            return _sb.ToString();
        }
    }
}
