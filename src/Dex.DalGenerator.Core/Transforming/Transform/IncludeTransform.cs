using System;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class IncludeTransform : RelationSelectedTransform
    {
        public IncludeTransform(Func<IEntityReferenceModel, bool> selector)
            : base(selector)
        {
        }

        public override void Transform(IEntityModel model, IEntityReferenceModel reference)
        {
            var referencedEntity = reference.TargetEntity;
            foreach (var property in referencedEntity.Properties.Values)
            {
                var attributes = reference.Attributes.Concat(property.Attributes).ToList();
                var p = new PropertyModel(property.PropertyType, reference.Name + property.Name, property.IsCollection, attributes);
                model.Properties.Add(p.Name, p);
            }

            foreach (var refer in referencedEntity.References.Values)
            {
                var r = new EntityReferenceModel(refer.TargetEntity, reference.Name + refer.Name, reference.DeclaringType,
                    refer.IsCollection, refer.Attributes, refer.CanWrite);

                model.References.Add(r.Name, r);
            }
        }
    }
}