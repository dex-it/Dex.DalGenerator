using System;
using System.Collections.Generic;
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
            _enums = entities
                .SelectMany(m => m.Properties.Values, (m, p) => p.PropertyType)
                .Where(p => p.BaseType == typeof(Enum)).Distinct()
                .ToArray();
        }

        public string Generate()
        {
            return TransformText();
        }
    }
}