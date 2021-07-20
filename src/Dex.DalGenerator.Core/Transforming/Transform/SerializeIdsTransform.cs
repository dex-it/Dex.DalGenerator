using System;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Core.Extensions;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class SerializeIdsTransform : RelationTransform
    {
        public override bool Selector(IEntityReferenceModel reference)
        {
            if (reference is null)
            {
                throw new System.ArgumentNullException(nameof(reference));
            }

            return reference.IsCollection && reference.TargetEntity.SourceType.IsSimple();
        }

        public override void Transform(IEntityModel model, IEntityReferenceModel reference)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (reference is null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            var propertyModel = new PropertyModel(null, typeof(string), reference.Name, false, reference.Attributes);
            model.Properties.Add(reference.Name, propertyModel);
        }
    }
}