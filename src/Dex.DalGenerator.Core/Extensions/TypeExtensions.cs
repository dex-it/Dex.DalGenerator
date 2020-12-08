using System;
using System.Linq;
using System.Reflection;
using Dex.Ef.Contracts.Entities;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool RootInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            return typeObj.GetTypeInfo().IsGenericType && typeObj.GetGenericTypeDefinition() == typeof(IEntity<>);
        }

        public static bool IsSimple(this Type type)
        {
            return PrimitiveParser.GetParser(type) != null;
        }

        public static string GetFriendlyName(this Type type, bool fullName = false)
        {
            if (type == typeof(int))
                return "int";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(string))
                return "string";
            else
            {
                var name = fullName ? type.FullName : type.Name;
                if (type.GetTypeInfo().IsGenericType)
                    return name.Split('`')[0] + "<" + string.Join(", ", type.GetTypeInfo().GetGenericArguments().Select(t => GetFriendlyName(t, fullName)).ToArray()) + ">";
                else
                    return name;
            }
        }


        public static bool IsNullable( this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        public static Type MakeNullable(this Type type)
        {
            if (type.GetTypeInfo().IsClass)
            {
                throw new InvalidOperationException(type.Name + " is not a struct.");
            }

            return typeof(Nullable<>).MakeGenericType(type);
        }
    }
}