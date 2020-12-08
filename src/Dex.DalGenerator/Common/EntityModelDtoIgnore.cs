using System;
using System.Collections.Generic;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Annotation.Attributes;

namespace Dex.DalGenerator.Common
{
    public class EntityModelDtoIgnore : IEntityModel
    {
        public EntityModelDtoIgnore(IEntityModel entityModel)
        {
            if (entityModel == null) throw new ArgumentNullException(nameof(entityModel));
            
            Attributes = entityModel.Attributes;
            Name = entityModel.Name;
            SourceType = entityModel.SourceType;
            Implements = entityModel.Implements;
            IsRootType = entityModel.IsRootType;
            Properties = entityModel.Properties
                .Where(p => p.Value.Attributes.Any(a => a.GetType().Name == nameof(CreateDtoAttribute)) == false)
                .ToDictionary(p => p.Key, p => p.Value);
            References = entityModel.References
                .Where(r => r.Value.Attributes.Any(a => a.GetType().Name == nameof(CreateDtoAttribute)) == false)
                .ToDictionary(r => r.Key, r => r.Value);
        }

        public IEnumerable<Attribute> Attributes { get; }
        public string Name { get; }
        public Type SourceType { get; set; }
        public List<Type> Implements { get; set; }
        public bool IsRootType { get; set; }
        public Dictionary<string, IPropertyModel> Properties { get; }
        public Dictionary<string, IEntityReferenceModel> References { get; }
    }
}