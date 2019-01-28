using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2Shadow.Exceptions
{
    internal static class ExHelpers
    {
        private static string GetVariableNameIinternal<T>(Expression<Func<T>> value)
        {
            if (value.Body is MemberExpression mExpr)
            {
                return mExpr.Member.Name;
            }

            throw new ArgumentException("Invalid expression. Member access expression is expected.");
        }

        public static void ThrowIfNull<T>(Expression<Func<T>> value)
        {
            var val = value.Compile()();
            var isNull = val?.GetType() == null;
            if (isNull)
            {
                throw new ArgumentNullException(GetVariableNameIinternal(value), ExMessages.argNull);
            }
        }

        public static void ThrowIfEmptyOrNull(Expression<Func<string>> value)
        {
            ThrowIfNull(value);

            var val = value.Compile()();
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException(ExMessages.stringEmpty, GetVariableNameIinternal(value));
            }
        }

        public static void ThrowIfSpacesOrNull(Expression<Func<string>> value)
        {
            ThrowIfEmptyOrNull(value);

            var val = value.Compile()();
            if (string.IsNullOrWhiteSpace(val))
            {
                throw new ArgumentException(ExMessages.stringWhitespace, GetVariableNameIinternal(value));
            }
        }

        public static void ThrowIfOneOfItemsIsNull<T>(Expression<Func<IEnumerable<T>>> value)
        {
            var val = value.Compile()();
            if (val.Any(x => x == null))
            {
                throw new ArgumentException(ExMessages.onOfItemsIsNull, GetVariableNameIinternal(value));
            }
        }
    }
}
