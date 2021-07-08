using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;
using Dex.Ef.Contracts.Entities;
using Microsoft.CSharp;

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
                return ToSimpleName(type);
            else if (type == typeof(short))
                return ToSimpleName(type);
            else if (type == typeof(byte))
                return ToSimpleName(type);
            else if (type == typeof(bool))
                return ToSimpleName(type);
            else if (type == typeof(long))
                return ToSimpleName(type);
            else if (type == typeof(float))
                return ToSimpleName(type);
            else if (type == typeof(double))
                return ToSimpleName(type);
            else if (type == typeof(decimal))
                return ToSimpleName(type);
            else if (type == typeof(string))
                return ToSimpleName(type);

            var name = fullName
                ? type.FullName
                : type.Name;

            if (type.GetTypeInfo().IsGenericType)
            {
                string args = string.Join(", ", type.GetTypeInfo().GetGenericArguments().Select(t => GetFriendlyName(t, fullName)).ToArray());

                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return $"{args}?";
                }
                else
                {
                    return name.Split('`')[0] + "<" + args + ">";
                }
            }
            else
            {
                return name;
            }
        }

        public static bool IsNullable( this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type MakeNullable(this Type type)
        {
            if (type.GetTypeInfo().IsClass)
            {
                throw new InvalidOperationException(type.Name + " is not a struct.");
            }

            return typeof(Nullable<>).MakeGenericType(type);
        }

        private static string ToSimpleName(Type type)
        {
            using (var provider = new CSharpCodeProvider())
            {
                var typeRef = new CodeTypeReference(type);
                string typeName = provider.GetTypeOutput(typeRef);
                return typeName;
            }
        }
    }
}