using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Dex.DalGenerator.Core
{
    public static class PrimitiveParser
    {
        public static Func<string, object> GetParser(Type type)
        {
            if (type == typeof(short))
            {
                return s => short.Parse(s);
            }

            if (type == typeof(int))
            {
                return s => IntParser(s);
            }

            if (type == typeof(double))
            {
                return s => DoubleParser(s);
            }

            if (type == typeof(decimal))
            {
                return s => DecimalParser(s);
            }

            if (type == typeof(string))
            {
                return s => s;
            }

            if (type == typeof(DateTime))
            {
                return s => s.EndsWith("Z") 
                    ? DateTime.Parse(s).ToUniversalTime() 
                    : DateTime.Parse(s);
            }

            if (type == typeof(DateTimeOffset))
            {
                return s => DateTimeOffset.Parse(s);
            }

            if (type == typeof(bool))
            {
                return s => bool.Parse(s);
            }

            if (type == typeof(Guid))
            {
                return s => Guid.Parse(s);
            }

            if (type.GetTypeInfo().IsEnum)
            {
                return s => Enum.Parse(type, s, true);
            }

            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var itemType = type.GetTypeInfo().GetGenericArguments().First();
                var subParser = GetParser(itemType);
                if (subParser != null)
                {
                    return s => string.IsNullOrWhiteSpace(s) ? null : subParser(s);
                }
            }

            return null;
        }
        
        private static int IntParser(string s)
        {
            if (!int.TryParse(s, out var result))
            {
                throw new Exception($"{s} is not a number.");
            }

            return result;
        }

        private static double DoubleParser(string s)
        {
            if (!double.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
            {
                throw new Exception($"{s} is not a double.");
            }

            return result;
        }

        private static decimal DecimalParser(string s)
        {
            if (!decimal.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
            {
                throw new Exception($"{s} is not a double.");
            }

            return result;
        }
    }
}