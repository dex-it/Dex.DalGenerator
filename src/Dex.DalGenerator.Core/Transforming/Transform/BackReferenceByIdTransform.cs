using System;
using System.Linq;
using System.Reflection;
using Dex.DalGenerator.Core.Contracts.Attributes;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Core.Extensions;
using Dex.DalGenerator.Annotation.Attributes;
using Dex.Ef.Attributes;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class BackReferenceByIdTransform : RelationSelectedTransform
    {
        public BackReferenceByIdTransform(Func<IEntityReferenceModel, bool> selector)
            : base(selector)
        {
        }

        public override void Transform(IEntityModel model, IEntityReferenceModel reference)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            var key = model.GetKey();
            if (key == null)
            {
                throw new InvalidOperationException($"Can't apply BackReferenceById to {model.Name} because it is not a root entity.");
            }

            var targetEntity = reference.TargetEntity;
            var attributes = reference.Attributes;

            attributes = attributes.Concat(new Attribute[] { new IndexAttribute(), new BackReferenceAttribute() }).ToList();

            if (!targetEntity.Attributes.OfType<IgnoreFksAttribute>().Any())
            {
                attributes = attributes.Concat(new[] { new RelationAttribute(model) }).ToList();
            }

            var propertyType = key.PropertyType;

            if (!propertyType.GetTypeInfo().IsGenericType || propertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                propertyType = typeof(Nullable<>).MakeGenericType(propertyType);
            }

            var prop = new PropertyModel(key.MemberInfo, propertyType, model.Name + "Id", attributes: attributes, canWrite: false);
            targetEntity.Properties.Add(prop.Name, prop);
        }
    }
}