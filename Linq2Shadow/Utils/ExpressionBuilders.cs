using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Linq2Shadow.Exceptions;

namespace Linq2Shadow.Utils
{
    /// <summary>
    /// Pack of builders to helps work with queries.
    /// </summary>
    public static class ExpressionBuilders
    {
        internal static readonly ParameterExpression DefaultRowParameter = Expression.Parameter(typeof(ShadowRow), "shadowRow");

        private static readonly MethodInfo RowIndexer = typeof(ShadowRow)
            .GetProperties()
            .Where(x => x.GetIndexParameters().Length == 1)
            .First()
            .GetMethod;

        private static MethodCallExpression Member(string member) =>
            Expression.Call(DefaultRowParameter, RowIndexer, Expression.Constant(member));

        /// <summary>
        /// Pack of the predicate builders and their helpers.
        /// </summary>
        public static class Predicates
        {
            #region are equals

            /// <summary>
            /// Build the equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreEquals(string member, bool value)
            {
                return AreEqualsInternal(member, value);
            }

            /// <summary>
            /// Build the equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreEquals(string member, string value)
            {
                return AreEqualsInternal(member, value);
            }

            /// <summary>
            /// Build the equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreEquals(string member, int value)
            {
                return AreEqualsInternal(member, value);
            }

            /// <summary>
            /// Build the equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreEquals(string member, DateTime value)
            {
                return AreEqualsInternal(member, value);
            }

            private static Expression<Func<ShadowRow, bool>> AreEqualsInternal<T>(string member, T value)
            {
                ExHelpers.ThrowIfSpacesOrNull(() => member);

                var indexerCall = Member(member);

                Expression eqExpr;

                var valConstExpr = Expression.Constant(value);
                var valueType = value?.GetType();
                if (valueType == null)
                {
                    eqExpr = Expression.Equal(indexerCall, valConstExpr);
                }
                else
                {
                    var convertExpr = Expression.Convert(indexerCall, valueType);
                    eqExpr = Expression.Equal(convertExpr, valConstExpr);
                }

                var lambda = Expression.Lambda<Func<ShadowRow, bool>>(eqExpr, DefaultRowParameter);
                return lambda;
            }

            #endregion are not equals

            #region are not equals

            /// <summary>
            /// Build the not equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreNotEquals(string member, bool value)
            {
                return AreNotEqualsInternal(member, value);
            }

            /// <summary>
            /// Build the not equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreNotEquals(string member, int value)
            {
                return AreNotEqualsInternal(member, value);
            }

            /// <summary>
            /// Build the not equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreNotEquals(string member, string value)
            {
                return AreNotEqualsInternal(member, value);
            }

            /// <summary>
            /// Build the not equality predicate.
            /// </summary>
            /// <typeparam name="T">Type of value to compare.</typeparam>
            /// <param name="member">Member which value will be compared.</param>
            /// <param name="value">Value to compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            public static Expression<Func<ShadowRow, bool>> AreNotEquals(string member, DateTime value)
            {
                return AreNotEqualsInternal(member, value);
            }

            private static Expression<Func<ShadowRow, bool>> AreNotEqualsInternal<T>(string member, T value)
            {
                ExHelpers.ThrowIfSpacesOrNull(() => member);

                var indexerCall = Member(member);

                Expression eqExpr;

                var valConstExpr = Expression.Constant(value);
                var valueType = value?.GetType();
                if (valueType == null)
                {
                    eqExpr = Expression.NotEqual(indexerCall, valConstExpr);
                }
                else
                {
                    var convertExpr = Expression.Convert(indexerCall, valueType);
                    eqExpr = Expression.NotEqual(convertExpr, valConstExpr);
                }

                var lambda = Expression.Lambda<Func<ShadowRow, bool>>(eqExpr, DefaultRowParameter);
                return lambda;
            }

            #endregion are not equals

            #region string predicates

            /// <summary>
            /// Build the string contains predicate.
            /// </summary>
            /// <param name="member">The member whose value should contain.</param>
            /// <param name="value">Value which should be matched.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> StringContains(string member, string value)
            {
                ExHelpers.ThrowIfSpacesOrNull(() => member);
                ExHelpers.ThrowIfNull(() => value);


                var memberCall = Member(member);
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

                var lambda = Expression.Lambda<Func<ShadowRow, bool>>(containsCall, DefaultRowParameter);
                return lambda;
            }

            /// <summary>
            /// Build a predicate of the string starts with.
            /// </summary>
            /// <param name="member">The member whose value should starts with.</param>
            /// <param name="value">Value which should be matched.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> StringStartsWith(string member, string value)
            {
                ExHelpers.ThrowIfSpacesOrNull(() => member);
                ExHelpers.ThrowIfNull(() => value);

                var memberCall = Member(member);
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

                var lambda = Expression.Lambda<Func<ShadowRow, bool>>(swCall, DefaultRowParameter);
                return lambda;
            }

            /// <summary>
            /// Build a predicate of the string ends with.
            /// </summary>
            /// <param name="member">The member whose value should ends with.</param>
            /// <param name="value">Value which should be matched.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="member"/> is null.</exception>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> StringEndsWith(string member, string value)
            {
                ExHelpers.ThrowIfSpacesOrNull(() => member);
                ExHelpers.ThrowIfNull(() => value);

                var memberCall = Member(member);
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

                var lambda = Expression.Lambda<Func<ShadowRow, bool>>(swCall, DefaultRowParameter);
                return lambda;
            }

            #endregion string predicates

            #region greater than

            /// <summary>
            /// Build the greater than predicate.
            /// </summary>
            /// <param name="member">The member whose value should greater than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> GreaterThan(string member, int value)
            {
                return GreaterThanInternal(member, value);
            }

            /// <summary>
            /// Build the greater than predicate.
            /// </summary>
            /// <param name="member">The member whose value should greater than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> GreaterThan(string member, DateTime value)
            {
                return GreaterThanInternal(member, value);
            }

            private static Expression<Func<ShadowRow, bool>> GreaterThanInternal<T>(string member, T value)
            {
                ExHelpers.ThrowIfNull(() => value);
                ExHelpers.ThrowIfSpacesOrNull(() => member);

                var valueType = value.GetType();

                if (valueType == typeof(string))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "String", "greater than"));
                }

                if (valueType == typeof(bool))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "Boolean", "greater than"));
                }

                var indexerCall = Member(member);

                var convertExpr = Expression.Convert(indexerCall, valueType);

                var gtExpr = Expression.GreaterThan(convertExpr, Expression.Constant(value));
                return Expression.Lambda<Func<ShadowRow, bool>>(gtExpr, DefaultRowParameter);
            }

            #endregion greater than

            #region greater than or equal

            /// <summary>
            /// Build the greater than or equal predicate.
            /// </summary>
            /// <param name="member">The member whose value should greater than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> GreaterThanOrEqual(string member, int value)
            {
                return GreaterThanOrEqualInternal(member, value);
            }

            /// <summary>
            /// Build the greater than or equal predicate.
            /// </summary>
            /// <param name="member">The member whose value should greater than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> GreaterThanOrEqual(string member, DateTime value)
            {
                return GreaterThanOrEqualInternal(member, value);
            }

            private static Expression<Func<ShadowRow, bool>> GreaterThanOrEqualInternal<T>(string member, T value)
            {
                ExHelpers.ThrowIfNull(() => value);
                ExHelpers.ThrowIfSpacesOrNull(() => member);

                var valueType = value.GetType();

                if (valueType == typeof(string))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "String", "greater than or equal"));
                }

                if (valueType == typeof(bool))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "Boolean", "greater than or equal"));
                }

                var indexerCall = Member(member);

                var convertExpr = Expression.Convert(indexerCall, valueType);

                var gtExpr = Expression.GreaterThanOrEqual(convertExpr, Expression.Constant(value));
                return Expression.Lambda<Func<ShadowRow, bool>>(gtExpr, DefaultRowParameter);
            }

            #endregion greater than or equal

            #region less than

            /// <summary>
            /// Build the less than predicate.
            /// </summary>
            /// <param name="member">The member whose value should less than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> LessThan(string member, int value)
            {
                return LessThanInternal(member, value);
            }

            /// <summary>
            /// Build the less than predicate.
            /// </summary>
            /// <param name="member">The member whose value should less than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> LessThan(string member, DateTime value)
            {
                return LessThanInternal(member, value);
            }

            private static Expression<Func<ShadowRow, bool>> LessThanInternal<T>(string member, T value)
            {
                ExHelpers.ThrowIfNull(() => value);
                ExHelpers.ThrowIfSpacesOrNull(() => member);

                var valueType = value.GetType();

                if (valueType == typeof(string))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "String", "less than"));
                }

                if (valueType == typeof(bool))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "Boolean", "less than"));
                }

                var indexerCall = Member(member);

                var convertExpr = Expression.Convert(indexerCall, valueType);

                var gtExpr = Expression.LessThan(convertExpr, Expression.Constant(value));
                return Expression.Lambda<Func<ShadowRow, bool>>(gtExpr, DefaultRowParameter);
            }

            #endregion less than

            #region less than or equal

            /// <summary>
            /// Build the less than or equal predicate.
            /// </summary>
            /// <param name="member">The member whose value should less than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> LessThanOrEqual(string member, int value)
            {
                return LessThanOrEqualInternal(member, value);
            }

            /// <summary>
            /// Build the less than or equal predicate.
            /// </summary>
            /// <param name="member">The member whose value should less than.</param>
            /// <param name="value">Value which should be compare.</param>
            /// <returns>Builded predicate.</returns>
            /// <exception cref="ArgumentException">Will throw when <paramref name="member"/> is whitespace.</exception>
            /// <exception cref="ArgumentNullException">Will throw when <paramref name="value"/> is null.</exception>
            public static Expression<Func<ShadowRow, bool>> LessThanOrEqual(string member, DateTime value)
            {
                return LessThanOrEqualInternal(member, value);
            }

            private static Expression<Func<ShadowRow, bool>> LessThanOrEqualInternal<T>(string member, T value)
            {
                ExHelpers.ThrowIfNull(() => value);
                ExHelpers.ThrowIfSpacesOrNull(() => member);

                var valueType = value.GetType();

                if (valueType == typeof(string))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "String", "less than or equal"));
                }

                if (valueType == typeof(bool))
                {
                    throw new InvalidOperationException(string.Format(ExMessages.invTypeComp, "Boolean", "less than or equal"));
                }

                var indexerCall = Member(member);

                var convertExpr = Expression.Convert(indexerCall, valueType);

                var gtExpr = Expression.LessThanOrEqual(convertExpr, Expression.Constant(value));
                return Expression.Lambda<Func<ShadowRow, bool>>(gtExpr, DefaultRowParameter);
            }

            #endregion less than or equal
        }

        public static Expression<Func<ShadowRow, bool>> And(Expression<Func<ShadowRow, bool>> left,
            Expression<Func<ShadowRow, bool>> right)
        {
            var binary = Expression.And(left.Body, right.Body);

            var p = DefaultRowParameter;
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

            var p = DefaultRowParameter;
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
    }
}
