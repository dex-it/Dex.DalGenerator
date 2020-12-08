using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Annotation.Attributes;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class PropertyModelExtensions
    {
        public static bool IsKey( this IPropertyModel property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            return property.HasAttribute<KeyAttribute>();
        }

        public static bool IsIdentity( this IPropertyModel property)
        {
            return property.HasAttribute<IdentityAttribute>();
        }

        public static bool IsNotNull( this IPropertyModel property)
        {
            var propertyType = property.PropertyType;
            var isValueType = propertyType.GetTypeInfo().IsValueType && !propertyType.IsNullable();
            return isValueType || property.HasAttribute<RequiredAttribute>();
        }
    }
}