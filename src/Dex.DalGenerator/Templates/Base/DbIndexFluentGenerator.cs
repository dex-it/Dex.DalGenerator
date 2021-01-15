using System;
using System.Collections.Generic;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Contracts.EntityModel;

// ReSharper disable once CheckNamespace
namespace Dex.DalGenerator.Templates
{
    public partial class DbIndexFluentGenerator : IEntityGenerator
    {
        private readonly IReadOnlyCollection<IEntityModel> _entities;
        private readonly string _namespace;
        private readonly string _entitiesNamespace;

        public DbIndexFluentGenerator(IEntityDomain dbDomain, string @namespace, string entitiesNamespace)
        {
            _entities = dbDomain.EntityModels ?? throw new ArgumentNullException(nameof(dbDomain));
            _namespace = @namespace;
            _entitiesNamespace = entitiesNamespace;
        }

        public string Generate()
        {
            return TransformText();
        }
    }
}