using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Contracts.EntityModel;

// ReSharper disable once CheckNamespace
namespace Dex.DalGenerator.Templates
{
    public partial class EnumFluentGenerator : IEntityGenerator
    {
        private readonly IReadOnlyCollection<Type> _enums;
        private readonly string _namespace;
        private readonly string[] _enumNamespaces;

        public EnumFluentGenerator(IEntityModel[] entities, string @namespace, string[] enumNamespaces)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            _namespace = @namespace;
            _enumNamespaces = enumNamespaces;

            _enums = GetEnums(entities.SelectMany(m => m.Properties.Values, (_, p) => p.PropertyType))
                .ToImmutableList();
        }

        public string Generate()
        {
            return TransformText();
        }

        private static IEnumerable<Type> GetEnums(IEnumerable<Type> propertyType)
        {
            foreach (var type in propertyType)
            {
                if (type.IsEnum)
                {
                    yield return type;
                }
                else
                {
                    Type? valueNullableType = Nullable.GetUnderlyingType(type);

                    if (valueNullableType != null && valueNullableType.IsEnum)
                    {
                        yield return valueNullableType;
                    }
                }
            }
        }
    }
}