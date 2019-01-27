using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Linq2Shadow.Utils;

namespace Linq2Shadow.QueryTranslators.Where
{
    internal class WhereQueryTranslator: QueryTranslator
    {
        private readonly WhereEjector whereEjector = new WhereEjector();

        private readonly Expression<Func<ShadowRow, bool>>[] _externalPredicates;

        internal WhereQueryTranslator(QueryParametersStore queryParametersStore,
                Expression<Func<ShadowRow, bool>>[] externalPredicates = null):
            base(queryParametersStore)
        {
            _externalPredicates = externalPredicates;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Visit(node.Body);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.OrElse:
                    _sb.Append("(");
                    Visit(node.Left);
                    _sb.Append(" OR ");
                    Visit(node.Right);
                    _sb.Append(")");
                    break;

                case ExpressionType.AndAlso:
                    _sb.Append("(");
                    Visit(node.Left);
                    _sb.Append(" AND ");
                    Visit(node.Right);
                    _sb.Append(")");
                    break;

                case ExpressionType.And:
                    Visit(node.Left);
                    _sb.Append(" AND ");
                    Visit(node.Right);
                    break;
                case ExpressionType.Or:
                    Visit(node.Left);
                    _sb.Append(" OR ");
                    Visit(node.Right);
                    break;

                case ExpressionType.GreaterThan:
                    Visit(node.Left);
                    _sb.Append(" > ");
                    Visit(node.Right);
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    Visit(node.Left);
                    _sb.Append(" >= ");
                    Visit(node.Right);
                    break;

                case ExpressionType.LessThan:
                    Visit(node.Left);
                    _sb.Append(" < ");
                    Visit(node.Right);
                    break;

                case ExpressionType.LessThanOrEqual:
                    Visit(node.Left);
                    _sb.Append(" <= ");
                    Visit(node.Right);
                    break;

                case ExpressionType.Equal:
                    {
                        if (ExpressionsInternalToolkit.TryGetConstant(node.Left, out var resValueLeft) &&
                            resValueLeft == null)
                        {
                            Visit(node.Right);
                            if (_notUnary)
                            {
                                _sb.Append(" IS NULL");
                            }
                        }
                        else if (ExpressionsInternalToolkit.TryGetConstant(node.Right, out var resValueRight) &&
                                 resValueRight == null)
                        {
                            Visit(node.Left);
                            _sb.Append(" IS NULL");
                        }
                        else
                        {
                            Visit(node.Left);
                            _sb.Append(" = ");
                            Visit(node.Right);
                        }
                    }
                    break;

                case ExpressionType.NotEqual:
                    {
                        object resValueLeft;
                        if (ExpressionsInternalToolkit.TryGetConstant(node.Left, out resValueLeft) &&
                            resValueLeft == null)
                        {
                            Visit(node.Right);
                            if (_notUnary)
                            {
                                _sb.Append(" IS NOT NULL");
                            }
                        }
                        else if (ExpressionsInternalToolkit.TryGetConstant(node.Right, out var resValueRight) &&
                                 resValueRight == null)
                        {
                            Visit(node.Left);
                            _sb.Append(" IS NOT NULL");
                        }
                        else
                        {
                            Visit(node.Left);
                            _sb.Append(" <> ");
                            Visit(node.Right);
                        }
                    }
                    break;
            }

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = ExpressionsInternalToolkit.GetConstant(node);
            var valueType = value?.GetType();

            var isCollection = valueType?.GetInterfaces()
                                   .Any(
                                       x => x == typeof(ICollection) ||
                                            (
                                                x.IsGenericType &&
                                                x.GetGenericTypeDefinition() == typeof(ICollection<>)
                                            )
                                   ) ?? false;

            if (isCollection)
            {
                var inSb = new StringBuilder();
                inSb.Append("(");

                var valuesCollection = value as IEnumerable;
                var enumerator = valuesCollection.GetEnumerator();

                var hasItems = enumerator.MoveNext();
                while (hasItems)
                {
                    var pName = _queryParamsStore.Append(enumerator.Current);
                    inSb.Append(pName);

                    var isLast = !enumerator.MoveNext();
                    if (isLast)
                    {
                        break;
                    }
                    else
                    {
                        inSb.Append(", ");
                    }
                }

                inSb.Append(")");

                _sb.Append(inSb);
            }
            else
            {
                var paramName = _queryParamsStore.Append(ExpressionsInternalToolkit.GetConstant(node));
                _sb.Append(paramName);
            }

            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // it is a Enumerable.Contains extension
            if(node.Method.DeclaringType == typeof(Enumerable) &&
                node.Method.Name == nameof(Enumerable.Contains))
            {
                if (node.Arguments.Count != 2)
                {
                    throw new InvalidOperationException("Constains call should be with 2 parameter.");
                }

                Visit(node.Arguments[1]);
                _sb.Append(_notUnary ? " NOT IN " : " IN ");
                Visit(node.Arguments[0]);

                return node;
            }

            // it is row indexer
            if (node.Method.DeclaringType == typeof(ShadowRow) && node.Method.Name == "get_Item")
            {
                if (node.Arguments.Count != 1)
                {
                    throw new InvalidOperationException("Only 1 indexer are supported.");
                }

                var member = ExpressionsInternalToolkit.GetConstant(node.Arguments[0]);
                _sb.Append(" ");
                _sb.Append(member);
                _sb.Append(" ");

                return node;
            }

            // it is a string manipulation methods
            if (node.Method.DeclaringType == typeof(string))
            {
                Visit(node.Object);

                if (node.Arguments.Count() != 1)
                {
                    throw new InvalidOperationException("Argument count is invalid.");
                }

                var value = ExpressionsInternalToolkit.GetConstant(node.Arguments[0]);

                if (value?.GetType() != typeof(string))
                {
                    throw new InvalidOperationException("Invalid value type.");
                }

                if (_notUnary)
                {
                    _sb.Append(" NOT");
                }

                if (node.Method.Name == nameof(String.Contains))
                {
                    var paramName = _queryParamsStore.Append($"%{value}%");
                    _sb.Append($" LIKE {paramName}");
                }
                else if (node.Method.Name == nameof(String.EndsWith))
                {
                    var paramName = _queryParamsStore.Append($"%{value}");
                    _sb.Append($" LIKE {paramName}");
                }
                else if (node.Method.Name == nameof(String.StartsWith))
                {
                    var paramName = _queryParamsStore.Append($"{value}%");
                    _sb.Append($" LIKE {paramName}");
                }
                else
                {
                    throw new Exception();
                }

                return node;
            }

            if (node.Method.Name == nameof(Boolean.Equals))
            {
                Visit(node.Object);

                var value = ExpressionsInternalToolkit.GetConstant(node.Arguments[0]);

                if (value?.GetType() == null)
                {
                    _sb.Append(_notUnary ? " IS NOT NULL" : " IS NULL ");
                }
                else if (value.GetType() == typeof(bool))
                {
                    var boolSqlValue = (bool)value ? 1 : 0;

                    _sb.Append(_notUnary ? " != " : " = ");
                    _sb.Append(_queryParamsStore.Append(boolSqlValue));
                }
                else
                {
                    _sb.Append(_notUnary ? " != " : " = ");
                    _sb.Append(_queryParamsStore.Append(value));
                }
            }

            return node;
        }

        private bool _notUnary = false;

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    _notUnary = true;
                    Visit(node.Operand);
                    _notUnary = false;
                    break;
                case ExpressionType.Convert:
                    Visit(node.Operand);
                    break;
            }

            return node;
        }

        public override string TranslateToSql(Expression expr)
        {
            whereEjector.Visit(expr);

            var p = ExpressionBuilders.DefaultRowParameter;

            var typedLambdas = whereEjector.WhereExpressions
                                           .Select(x => Expression.Lambda<Func<ShadowRow, bool>>(x.Body, p))
                                           .ToList();

            if (_externalPredicates?.Any() ?? false)
            {
                typedLambdas.AddRange(_externalPredicates);
            }

            if (typedLambdas.Any())
            {
                var exp = ExpressionBuilders.And(typedLambdas.ToArray());
                Visit(exp);
            }

            var whereSql = _sb.ToString();
            return whereSql;
        }
    }
}
