using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Linq2Shadow
{
    internal static class ExpressionsInternalToolkit
    {
        public static Expression SkipUnary(Expression source)
        {
            if (source is UnaryExpression expr) return expr.Operand;
            return source;
        }

        public static object GetConstant(Expression expr)
        {
            if (TryGetConstant(expr, out var result))
            {
                return result;
            }

            throw new InvalidOperationException("Invalid expression.");
        }

        public static bool TryGetConstant(Expression expr, out object value)
        {
            expr = SkipUnary(expr);
            value = default(object);

            // check to compiler generated closure
            if (expr is ConstantExpression cExpr)
            {
                var cExptValType = cExpr.Value?.GetType();
                if (cExptValType != null && Attribute.IsDefined(cExptValType, typeof(CompilerGeneratedAttribute)))
                {
                    var generatedField = cExptValType.GetFields().FirstOrDefault();
                    if (generatedField != null)
                    {
                        value = generatedField.GetValue(cExpr.Value);
                        return true;
                    }
                }
                else // it is a simple literal
                {
                    value = cExpr.Value;
                    return true;
                }
            }

            // check to user defined closure
            if (expr is MemberExpression mValExpr && mValExpr.Expression is ConstantExpression mcValExpr)
            {
                var objectMember = Expression.Convert(mValExpr, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();
                var val = getter();
                value = val;
                return true;
            }

            {
                bool compliedLambdaIsExecuted = false;
                object gettedFromLambda = null;
                try
                {
                    var objectMember = Expression.Convert(expr, typeof(object));
                    var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                    var getter = getterLambda.Compile();
                    gettedFromLambda = getter();

                    compliedLambdaIsExecuted = true;
                }
                catch { }
                if (compliedLambdaIsExecuted)
                {
                    value = gettedFromLambda;
                    return true;
                }
            }

            return false;
        }

        public static bool IsCountQueryableCall(Expression expr)
        {
            return expr is MethodCallExpression mCallExpr &&
                   mCallExpr.Method.DeclaringType == typeof(Queryable) &&
                   mCallExpr.Method.Name == nameof(Queryable.Count);
        }

        public static bool IsFirstQueryableCall(Expression expr)
        {
            return expr is MethodCallExpression mCallExpr &&
                   mCallExpr.Method.DeclaringType == typeof(Queryable) &&
                   mCallExpr.Method.Name == nameof(Queryable.First);
        }

        public static bool IsFirstOrDefaultQueryableCall(Expression expr)
        {
            return expr is MethodCallExpression mCallExpr &&
                   mCallExpr.Method.DeclaringType == typeof(Queryable) &&
                   mCallExpr.Method.Name == nameof(Queryable.FirstOrDefault);
        }
    }
}
