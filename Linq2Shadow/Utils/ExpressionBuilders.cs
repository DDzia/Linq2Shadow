using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Linq2Shadow.Utils
{
    /// <summary>
    /// Pack of builders to helps work with queries.
    /// </summary>
    public static partial class ExpressionBuilders
    {
        internal static readonly ParameterExpression DefaultRowParameter = Expression.Parameter(typeof(ShadowRow), "row");

        private static readonly MethodInfo RowIndexer = typeof(ShadowRow)
            .GetProperties()
            .First(x => x.GetIndexParameters().Length == 1)
            .GetMethod;

        private static MethodCallExpression MemberInternal(string member) =>
            Expression.Call(DefaultRowParameter, RowIndexer, Expression.Constant(member));

        /// <summary>
        /// Build member access expression.
        /// </summary>
        /// <param name="memberName">Member name.</param>
        /// <returns>Builded expression.</returns>
        public static Expression<Func<ShadowRow, object>> MemberAccess(string memberName)
        {
            if(memberName is null)
                throw new ArgumentNullException(nameof(memberName));

            if(string.IsNullOrWhiteSpace(memberName))
                throw new ArgumentException(nameof(memberName));

            var callExpr = MemberInternal(memberName);
            return Expression.Lambda<Func<ShadowRow, object>>(callExpr, DefaultRowParameter);
        }
    }
}
