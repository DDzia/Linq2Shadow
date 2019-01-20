using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Linq2Shadow.Exceptions;

namespace Linq2Shadow.Utils
{
    public static class ExpressionUtils
    {
        private static readonly MethodInfo ObjectEquals =
            typeof(object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public);

        private static readonly MethodInfo RowIndexer = typeof(ShadowRow)
            .GetProperties().Where(x => x.GetIndexParameters().Length == 1).First().GetMethod;

        private static MethodCallExpression IndexCall(ParameterExpression p, string member) =>
            Expression.Call(p, RowIndexer, Expression.Constant(member));

        public static ParameterExpression CreateDefaultRowParameter() => Expression.Parameter(typeof(ShadowRow), "x");

        public static Expression<Func<ShadowRow, bool>> MakeEquals<T>(string member, T value, ParameterExpression parametr = null)
        {
            ExHelpers.ThrowIfSpacesOrNull(() => member);

            return MakeEquals(member, Expression.Constant(value), parametr);
        }

        public static Expression<Func<ShadowRow, bool>> MakeEquals(string member, ConstantExpression value, ParameterExpression parametr = null)
        {
            var p = parametr ?? CreateDefaultRowParameter();

            var indexerCall = IndexCall(p, member);

            Expression eqExpr;

            var valueType = value.Value?.GetType();
            if (valueType == null)
            {
                eqExpr = Expression.Equal(indexerCall, value);
            }
            else
            {
                var convertExpr = Expression.Convert(indexerCall, valueType);
                eqExpr = Expression.Equal(convertExpr, value);
            }

            var lambda = Expression.Lambda<Func<ShadowRow, bool>>(eqExpr, p);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> MakeNotEquals(string member, ConstantExpression value, ParameterExpression parametr = null)
        {
            var p = parametr ?? CreateDefaultRowParameter();

            var indexerCall = IndexCall(p, member);

            Expression notEqualExp;

            var valueType = value.Value?.GetType();
            if (valueType == null)
            {
                notEqualExp = Expression.NotEqual(indexerCall, value);
            }
            else
            {
                var convertExpr = Expression.Convert(indexerCall, valueType);
                notEqualExp = Expression.NotEqual(convertExpr, value);
            }

            var lambda = Expression.Lambda<Func<ShadowRow, bool>>(notEqualExp, p);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> MakeGreaterThan(string member, ConstantExpression value,
            ParameterExpression parametr = null)
        {
            var p = parametr ?? CreateDefaultRowParameter();

            var indexerCall = IndexCall(p, member);

            var valueType = value.Value?.GetType();
            if (valueType == null)
            {
                throw new InvalidOperationException("NULL");
            }

            var convertExpr = Expression.Convert(indexerCall, valueType);

            var gtExpr = Expression.GreaterThan(convertExpr, value);
            return Expression.Lambda<Func<ShadowRow, bool>>(gtExpr);
        }

        public static Expression<Func<ShadowRow, bool>> MakeGreaterThanOrEqual(string member, ConstantExpression value,
            ParameterExpression parametr = null)
        {
            var p = parametr ?? CreateDefaultRowParameter();

            var indexerCall = IndexCall(p, member);

            var valueType = value.Value?.GetType();
            if (valueType == null)
            {
                throw new InvalidOperationException("NULL");
            }

            var convertExpr = Expression.Convert(indexerCall, valueType);

            var gtExpr = Expression.GreaterThanOrEqual(convertExpr, value);
            return Expression.Lambda<Func<ShadowRow, bool>>(gtExpr);
        }

        public static Expression<Func<ShadowRow, bool>> MakeLessThan(string member, ConstantExpression value,
            ParameterExpression parametr = null)
        {
            var p = parametr ?? CreateDefaultRowParameter();

            var indexerCall = IndexCall(p, member);

            var valueType = value.Value?.GetType();
            if (valueType == null)
            {
                throw new InvalidOperationException("NULL");
            }

            var convertExpr = Expression.Convert(indexerCall, valueType);

            var lessExpr = Expression.LessThan(convertExpr, value);
            return Expression.Lambda<Func<ShadowRow, bool>>(lessExpr);
        }

        public static Expression<Func<ShadowRow, bool>> MakeLessThanOrEqual(string member, ConstantExpression value,
            ParameterExpression parametr = null)
        {
            var p = parametr ?? CreateDefaultRowParameter();

            var indexerCall = IndexCall(p, member);

            var valueType = value.Value?.GetType();
            if (valueType == null)
            {
                throw new InvalidOperationException("NULL");
            }

            var convertExpr = Expression.Convert(indexerCall, valueType);

            var lessExpr = Expression.LessThanOrEqual(convertExpr, value);
            return Expression.Lambda<Func<ShadowRow, bool>>(lessExpr);
        }

        public static Expression<Func<ShadowRow, bool>> And(Expression<Func<ShadowRow, bool>> left,
            Expression<Func<ShadowRow, bool>> right)
        {
            var binary = Expression.And(left.Body, right.Body);

            var p = CreateDefaultRowParameter();
            var lambda = Expression.Lambda<Func<ShadowRow, bool>>(binary, p);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> And(Expression<Func<ShadowRow, bool>>[] predicates)
        {
            if (predicates == null)
                throw new ArgumentNullException(nameof(predicates));

            if (!predicates.Any())
                throw new ArgumentException(nameof(predicates));

            if (predicates.Length == 1) return predicates[0];

            Expression<Func<ShadowRow, bool>> result = predicates[1];

            foreach (var expression in predicates.Skip(1))
            {
                result = And(result, expression);
            }

            return result;
        }

        public static Expression<Func<ShadowRow, bool>> Or(Expression<Func<ShadowRow, bool>> left,
            Expression<Func<ShadowRow, bool>> right)
        {
            var binary = Expression.Or(left.Body, right.Body);

            var p = CreateDefaultRowParameter();
            var lambda = Expression.Lambda<Func<ShadowRow, bool>>(binary, p);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> Or(Expression<Func<ShadowRow, bool>>[] predicates)
        {
            if (predicates == null)
                throw new ArgumentNullException(nameof(predicates));

            if (!predicates.Any())
                throw new ArgumentException(nameof(predicates));

            if (predicates.Length == 1) return predicates[0];

            Expression<Func<ShadowRow, bool>> result = predicates[1];

            foreach (var expression in predicates.Skip(1))
            {
                result = Or(result, expression);
            }

            return result;
        }

        public static Expression<Func<ShadowRow, bool>> Contains(string member,
            string value,
            ParameterExpression parametr = null)
        {
            if (string.IsNullOrEmpty(member))
                throw new ArgumentException(nameof(member));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(nameof(value));

            parametr = parametr ?? CreateDefaultRowParameter();

            var memberCall = IndexCall(parametr, member);
            var memberAsString = Expression.Convert(memberCall, typeof(string));


            var mi = typeof(string).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x =>
                {
                    var pms = x.GetParameters();
                    return x.Name == nameof(string.Contains) &&
                           pms.Length == 1 &&
                           pms[0].ParameterType == typeof(string);
                })
                .First();
            var containsCall = Expression.Call(memberAsString, mi, Expression.Constant(value));

            parametr = parametr ?? CreateDefaultRowParameter();
            var lambda = Expression.Lambda<Func<ShadowRow, bool>>(containsCall, parametr);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> StartsWith(string member,
            string value,
            ParameterExpression parametr = null)
        {
            if (string.IsNullOrEmpty(member))
                throw new ArgumentException(nameof(member));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(nameof(value));

            parametr = parametr ?? CreateDefaultRowParameter();

            var memberCall = IndexCall(parametr, member);
            var memberAsString = Expression.Convert(memberCall, typeof(string));


            var mi = typeof(string).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x =>
                {
                    var pms = x.GetParameters();
                    return x.Name == nameof(string.StartsWith) &&
                           pms.Length == 1 &&
                           pms[0].ParameterType == typeof(string);
                })
                .First();
            var swCall = Expression.Call(memberAsString, mi, Expression.Constant(value));

            parametr = parametr ?? CreateDefaultRowParameter();
            var lambda = Expression.Lambda<Func<ShadowRow, bool>>(swCall, parametr);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> EndsWith(string member,
            string value,
            ParameterExpression parametr = null)
        {
            if (string.IsNullOrEmpty(member))
                throw new ArgumentException(nameof(member));

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(nameof(value));

            parametr = parametr ?? CreateDefaultRowParameter();

            var memberCall = IndexCall(parametr, member);
            var memberAsString = Expression.Convert(memberCall, typeof(string));


            var mi = typeof(string).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                   .Where(x =>
                                   {
                                       var pms = x.GetParameters();
                                       return x.Name == nameof(string.EndsWith) &&
                                              pms.Length == 1 &&
                                              pms[0].ParameterType == typeof(string);
                                   })
                                   .First();
            var swCall = Expression.Call(memberAsString, mi, Expression.Constant(value));

            parametr = parametr ?? CreateDefaultRowParameter();
            var lambda = Expression.Lambda<Func<ShadowRow, bool>>(swCall, parametr);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> CollectionContains(ConstantExpression data, string member)
        {
            var constValue = ExpressionsInternalToolkit.GetConstant(data);
            if (constValue == null)
                throw new InvalidOperationException("Collection constant can't be NULL.");

            if(!(constValue is IEnumerable))
                throw new InvalidOperationException("Invalid collection.");

            var list = new List<object>();
            foreach (var c in (constValue as IEnumerable))
            {
                list.Add(c);
            }

            Expression<Func<ShadowRow, bool>> lambda = x => Enumerable.Contains(list, x[member]);
            return lambda;
        }

        public static Expression<Func<ShadowRow, bool>> MemberPropertyAccess(string propertyName)
        {
            ExHelpers.ThrowIfNull(() => propertyName);
            // propertyName = propertyName ?? throw new ArgumentNullException()
            return null;
        }
    }
}
