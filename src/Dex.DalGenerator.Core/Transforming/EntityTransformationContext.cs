using System;
using System.Collections.Generic;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.Transforming.Transform;

namespace Dex.DalGenerator.Core.Transforming
{
    public class EntityTransformationContext
    {
        public Func<IEntityModel, bool> EntitySelector { get; }
        public IReadOnlyCollection<PropertyTransform> PropTransformations { get; set; }
        public IReadOnlyCollection<RelationTransform> RelTransforms { get; set; }
        public IPropertyModel[] AddProps { get; set; }
        public Action<IEntityModel> EntityTransform { get; set; }

        public EntityTransformationContext(Func<IEntityModel, bool> entitySelector)
        {
            EntitySelector = entitySelector ?? throw new ArgumentNullException(nameof(entitySelector));
        }
    }
}