using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Annotation.Attributes;
using System.Diagnostics;
using Dex.DalGenerator.Core.Helpers;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class PropertyModelExtensions
    {
        public static bool IsKey(this IPropertyModel property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            return property.HasAttribute<KeyAttribute>();
        }

        public static bool IsIdentity(this IPropertyModel property)
        {
            return property.HasAttribute<IdentityAttribute>();
        }

        public static bool IsNotNull(this IPropertyModel property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var propertyType = property.PropertyType;
            var isValueType = propertyType.GetTypeInfo().IsValueType && !propertyType.IsNullable();
            return isValueType || property.HasAttribute<RequiredAttribute>();
        }

        /// <summary>
        /// Возвращает '?' если ссылочный тип поддерживает значение <see langword="null"/>.
        /// </summary>
        /// <returns>'?' или <see langword="null"/>.</returns>
        public static char? GetNullableRefChar(this IPropertyModel property)
        {
            Debug.Assert(property != null);

            if (property.MemberInfo != null && (!property.PropertyType.IsValueType || property.IsCollection))
            {
                bool isNonNullable = NullableRefConvention.IsNonNullableReferenceType(property.MemberInfo);
                return isNonNullable ? null : '?';
            }
            else
            {
                return null;
            }
        }

        public static string SuppressNullableRef(this IPropertyModel property)
        {
            Debug.Assert(property != null);

            if (property.MemberInfo != null && (!property.PropertyType.IsValueType || property.IsCollection))
            {
                bool isNonNullable = NullableRefConvention.IsNonNullableReferenceType(property.MemberInfo);
                return isNonNullable ? " = null!;" : "";
            }
            else
            {
                return "";
            }
        }
    }
}