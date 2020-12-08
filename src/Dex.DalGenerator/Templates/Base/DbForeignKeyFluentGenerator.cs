using System;
using System.Linq;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Generator;

// ReSharper disable once CheckNamespace
namespace Dex.DalGenerator.Templates
{
    public partial class DbForeignKeyFluentGenerator : IEntityGenerator
    {
        private readonly Relation[] _relations;
        private readonly string _namespace;
        private readonly string _entitiesNamespace;

        public DbForeignKeyFluentGenerator(Relation[] relations, string @namespace, string entitiesNamespace)
        {
            _relations = relations ?? throw new ArgumentNullException(nameof(relations));
            _namespace = @namespace;
            _entitiesNamespace = entitiesNamespace;
        }

        private Relation GetBackRelation(Relation relation)
        {
            return _relations
                .SingleOrDefault(r => r.IsBackRelation &&
                                      r.TypeName == relation.EntityName &&
                                      r.KeyPropertyName == relation.KeyPropertyName);
        }

        public string Generate()
        {
            return TransformText();
        }
    }
}