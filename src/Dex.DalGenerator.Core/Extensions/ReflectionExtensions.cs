using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<T> GetAttributes<T>(this MemberInfo memberInfo, bool inherit = true)
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        public static string DeleteInterfacePrefix(this string interfaceTypeName)
        {
            if (interfaceTypeName == null || !interfaceTypeName.StartsWith("I"))
            {
                return interfaceTypeName;
            }

            var deleteInterfacePrefix = interfaceTypeName.Substring(1);
            return deleteInterfacePrefix;
        }

        public static string ToCodeString(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                var gType = type.GetGenericTypeDefinition();
                return gType.Name.Remove(gType.Name.IndexOf('`'))
                       + "<"
                       + string.Join(",", type.GetTypeInfo().GetGenericArguments().Select(ToCodeString))
                       + ">";
            }
            return type.Name;
        }

        public static string ToCodeStringFull(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                var gType = type.GetGenericTypeDefinition();
                return gType.FullName.Remove(gType.FullName.IndexOf('`'))
                       + "<"
                       + string.Join(",", type.GetTypeInfo().GetGenericArguments().Select(ToCodeStringFull))
                       + ">";
            }
            return type.FullName;
        }

        public static bool InheritFromInterface<TInterface>(this Type type)
        {
            var value = typeof(TInterface);
            if (!value.GetTypeInfo().IsInterface) throw new ArgumentException("type must be interface");
            return type.GetTypeInfo().GetInterfaces().Contains(value);
        }

        public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this Type type)
        {
            var attributeType = typeof(T);
            return
                type.GetTypeInfo().GetCustomAttributes(attributeType, true)
                    .Union(
                        type.GetTypeInfo().GetInterfaces()
                            .SelectMany(interfaceType => interfaceType.GetTypeInfo().GetCustomAttributes(attributeType, true)))
                    .Distinct()
                    .Cast<T>();
        }
#if !CORE
        public static IEnumerable<T> GetCustomAttributesIncludingBaseInterfaces<T>(this PropertyInfo property)
        {
            var attributeType = typeof(T);
            var baseType = property.ReflectedType;
            return
                property.GetCustomAttributes(attributeType, true)
                    .Union(
                        baseType.GetInterfaces()
                            .SelectMany(
                                interfaceType =>
                                    interfaceType.GetProperties()
                                        .Where(interfaceProperty => interfaceProperty.Name == property.Name)
                                        .SelectMany(
                                            interfaceProperty =>
                                                    interfaceProperty.GetCustomAttributes(attributeType, true))))
                    .Distinct()
                    .Cast<T>();
        }
#endif
    }
}