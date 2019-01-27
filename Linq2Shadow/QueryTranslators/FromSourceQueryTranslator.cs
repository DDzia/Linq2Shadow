﻿using System;
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

            var itIsCount = false;
            var itIsFirst = false;
            var skipUsed = ExpressionsInternalToolkit.SkipIsUsed(expr);
            var takeUsed = ExpressionsInternalToolkit.TakeIsUsed(expr);

#pragma warning disable CS0642

            if (itIsCount = ExpressionsInternalToolkit.IsCountQueryableCall(expr)) ;
            else if (itIsFirst = ExpressionsInternalToolkit.IsFirstQueryableCall(expr) ||
                                 ExpressionsInternalToolkit.IsFirstOrDefaultQueryableCall(expr)) ;
#pragma warning restore CS0642


            if (itIsCount)
            {
                _sb.Append("SELECT COUNT (*) FROM ");
                var mCall = expr as MethodCallExpression;
                if (mCall.Arguments.Count == 2)
                {
                    var lambda = ExpressionsInternalToolkit.SkipUnary(mCall.Arguments[1]) as LambdaExpression; // skip Quote
                    var lambdaTyped =
                        Expression.Lambda<Func<ShadowRow, bool>>(lambda.Body, ExpressionBuilders.DefaultRowParameter);
                    externalPredicates.Add(lambdaTyped);
                }
            }
            else if (itIsFirst && !skipUsed)
            {
                _sb.Append("SELECT TOP 1 * FROM ");
                var mCall = expr as MethodCallExpression;
                if (mCall.Arguments.Count == 2)
                {
                    var lambda = ExpressionsInternalToolkit.SkipUnary(mCall.Arguments[1]) as LambdaExpression; // skip Quote
                    var lambdaTyped =
                        Expression.Lambda<Func<ShadowRow, bool>>(lambda.Body, ExpressionBuilders.DefaultRowParameter);
                    externalPredicates.Add(lambdaTyped);

                }
            }
            else
            {
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

            if (!itIsCount)
            {
                var orderByLine = new OrderByQueryTranslator(_queryParamsStore)
                    .TranslateToSql(expr);
                var orderByUsed = !string.IsNullOrWhiteSpace(orderByLine);
                if (orderByUsed)
                {
                    _sb.Append(" ORDER BY ");
                    _sb.Append(orderByLine);
                }

                if (skipUsed)
                {
                    if (!orderByUsed)
                    {
                        _sb.Append(" ORDER BY (SELECT NULL)");
                    }

                    _sb.Append(" OFFSET ");
                    var skipCount = ExpressionsInternalToolkit.GetSkipCount(expr);
                    _sb.Append(_queryParamsStore.Append(skipCount));
                    _sb.Append(" ROWS");
                }

                if (takeUsed)
                {
                    if (!skipUsed)
                    {
                        if (!orderByUsed)
                        {
                            _sb.Append(" ORDER BY (SELECT NULL)");
                        }

                        _sb.Append(" OFFSET 0 ROWS");
                    }

                    _sb.Append(" FETCH NEXT ");
                    var takeCount = ExpressionsInternalToolkit.GetTakeCount(expr);
                    _sb.Append(_queryParamsStore.Append(takeCount));
                    _sb.Append(" ROW ONLY");
                }
            }

            if (itIsFirst && skipUsed)
            {
                _sb.Append(" FETCH NEXT 1 ROW ONLY");
            }

            return _sb.ToString();
        }
    }
}
