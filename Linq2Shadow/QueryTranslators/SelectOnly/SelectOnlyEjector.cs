using System.Linq.Expressions;
using Linq2Shadow.Extensions;

namespace Linq2Shadow.QueryTranslators.SelectOnly
{
    internal class SelectOnlyEjector: ExpressionVisitor
    {
        public string[] FieldNames { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(QueryableExtensions) &&
                node.Method.Name == nameof(QueryableExtensions.SelectOnly))
            {
                // use last operator call only
                FieldNames = FieldNames
                             ?? (string[])ExpressionsInternalToolkit.GetConstant(node.Arguments[1]);
            }

            return base.VisitMethodCall(node);
        }
    }
}
