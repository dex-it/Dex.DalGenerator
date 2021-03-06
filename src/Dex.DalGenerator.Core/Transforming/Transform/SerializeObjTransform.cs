using System;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Annotation.Attributes;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class SerializeObjTransform : RelationSelectedTransform
    {
        public SerializeObjTransform(Func<IEntityReferenceModel, bool> selector)
            : base(selector)
        {
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

            var name = reference.Name;
            var attributes = reference.Attributes.ToList();
            attributes.Add(new SerializeIntoStringFieldAttribute());

            model.Properties.Add(name, new PropertyModel(null, typeof(string), name, attributes: attributes));
        }
    }
}