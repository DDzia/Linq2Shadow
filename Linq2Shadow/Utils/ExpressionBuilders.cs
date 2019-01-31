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
            .Where(x => x.GetIndexParameters().Length == 1)
            .First()
            .GetMethod;

        private static MethodCallExpression Member(string member) =>
            Expression.Call(DefaultRowParameter, RowIndexer, Expression.Constant(member));
    }
}
