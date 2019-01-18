using System;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2Shadow.QueryTranslators.OrderBy
{
    internal class OrderByQueryTranslator: QueryTranslator
    {
        private readonly OrderByEjector _ejector = new OrderByEjector();

        internal OrderByQueryTranslator(QueryParametersStore queryParametersStore)
            : base(queryParametersStore) { }


        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Visit(node.Body);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object is ParameterExpression)
            {
                var arg = node.Arguments.FirstOrDefault()
                    ?? throw new ArgumentException("Invalid argument.");
                var memberName = ExpressionsInternalToolkit.GetConstant(arg);

                _sb.Append(memberName);
            }

            return node;
        }

        public override string TranslateToSql(Expression expr)
        {
            _ejector.Visit(expr);

            var index = 0;
            foreach (var expression in _ejector.OrderByExpressions)
            {
                _sb.Append(" ");
                Visit(expression.Item1);
                _sb.Append(" ");
                _sb.Append(expression.Item2 ? "ASC" : "DESC");

                var noLast = index != _ejector.OrderByExpressions.Count - 1;
                if (noLast)
                {
                    _sb.Append(", ");
                }

                index++;
            }

            var sql =  _sb.ToString();
            return sql;
        }
    }
}
