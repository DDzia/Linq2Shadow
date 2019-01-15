using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExpToSql.Visitors
{
    static class SqlHelpers
    {
        public static string ToSqlValueAsString(object value)
        {
            var valueType = value?.GetType();

            if (valueType == null) return "NULL";

            if (valueType == typeof(string)) return $"'{value}'";

            string EnumerationToStr(IEnumerable enumeration)
            {
                var strItems = new List<string>();
                foreach (var item in enumeration)
                {
                    strItems.Add(ToSqlValueAsString(item));
                }
                return $"[{string.Join(", ", strItems)}]";
            }

            var ienumerableGenericArg = valueType.GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetGenericArguments()[0])
                .FirstOrDefault();

            if (valueType.IsArray || ienumerableGenericArg != null)
            {
                return EnumerationToStr(value as IEnumerable);
            }


            if (valueType == typeof(int)) return value.ToString();

            if (valueType == typeof(double)) return ((double) value).ToString(CultureInfo.InvariantCulture);
            if (valueType == typeof(decimal)) return ((double)value).ToString(CultureInfo.InvariantCulture);
            if (valueType == typeof(float)) return ((double)value).ToString(CultureInfo.InvariantCulture);

            throw new InvalidOperationException($"Unsupported value type: '{valueType.Name}'");
        }
    }
}
