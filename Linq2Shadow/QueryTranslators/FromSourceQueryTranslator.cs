using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Linq2Shadow.QueryTranslators.OrderBy;
using Linq2Shadow.QueryTranslators.Where;
using Linq2Shadow.Utils;

namespace Linq2Shadow.QueryTranslators
{
    internal sealed class FromSourceQueryTranslator: QueryTranslator
    {
        private readonly string _sourceName;

        internal FromSourceQueryTranslator(QueryParametersStore queryParamsStore,
            string sourceName)
        : base(queryParamsStore)
        {
            _sourceName = sourceName;
        }

        public override string TranslateToSql(Expression expr)
        {
            List<Expression<Func<ShadowRow, bool>>> externalPredicates = new List<Expression<Func<ShadowRow, bool>>>();

            var itIsQuery = false;

            if (ExpressionsInternalToolkit.IsCountQueryableCall(expr))
            {
                _sb.Append("SELECT COUNT (*) FROM ");
                var mCall = expr as MethodCallExpression;
                if (mCall.Arguments.Count == 2)
                {
                    var lambda = ExpressionsInternalToolkit.SkipUnary(mCall.Arguments[1]) as LambdaExpression; // skip Quote
                    var lambdaTyped =
                        Expression.Lambda<Func<ShadowRow, bool>>(lambda.Body, ExpressionUtils.CreateDefaultRowParameter());
                    externalPredicates.Add(lambdaTyped);

                }
            }
            else
            {
                itIsQuery = true;
                _sb.Append("SELECT * FROM ");
            }

            _sb.Append(_sourceName);

            var whereLine = new WhereQueryTranslator(_queryParamsStore,
                    externalPredicates.Any() ? externalPredicates.ToArray() : null)
                .TranslateToSql(expr);
            if (!string.IsNullOrWhiteSpace(whereLine))
            {
                _sb.Append(" WHERE ");
                _sb.Append(whereLine);
            }

            // apply ordering for queries only
            if (itIsQuery)
            {
                var orderByLine = new OrderByQueryTranslator(_queryParamsStore)
                    .TranslateToSql(expr);
                if (!string.IsNullOrWhiteSpace(orderByLine))
                {
                    _sb.Append(" ORDER BY ");
                    _sb.Append(orderByLine);
                }
            }

            return _sb.ToString();
        }
    }
}
