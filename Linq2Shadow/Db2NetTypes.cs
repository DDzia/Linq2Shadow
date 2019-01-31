using System;
using System.Collections.Generic;
using System.Linq;

namespace Linq2Shadow
{
    internal class Db2NetTypes
    {
        private static readonly IReadOnlyCollection<Type> SupportedTypes = new List<Type>(new[]
        {
            typeof(bool),
            typeof(byte),
            // typeof(char), -- really unsupported
            typeof(DateTime),
            typeof(decimal),
            typeof(double),
            // typeof(float),  -- really unsupported
            typeof(Guid),
            typeof(string),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64)
        });

        public static bool IsSupportedType(Type type)
        {
            if(type == null) return true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GenericTypeArguments[0];
            }

            return SupportedTypes.Contains(type);
        }
    }
}
