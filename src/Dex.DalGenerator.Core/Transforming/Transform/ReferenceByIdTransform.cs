using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Dex.DalGenerator.Core.Contracts.Attributes;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Core.Extensions;
using Dex.Ef.Attributes;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class ReferenceByIdTransform : RelationSelectedTransform
    {
        public ReferenceByIdTransform(Func<IEntityReferenceModel, bool> selector)
            : base(selector)
        {
        }

        public override void Transform(IEntityModel model, IEntityReferenceModel reference)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            var target = reference.TargetEntity;
            var key = target.GetKey();
            if(key == null)
                throw new InvalidOperationException("Target must contain key");
            

            var attrs = AddAttribute(reference);

            var propertyType = key.PropertyType;
            if (propertyType.GetTypeInfo().IsValueType)
            {
                var requiredAttribute = reference.Attributes.OfType<RequiredAttribute>().FirstOrDefault();
                if (requiredAttribute == null)
                {
                    propertyType = propertyType.MakeNullable();
                }
            }

            var propertyModel = new PropertyModel(key.MemberInfo, propertyType, reference.Name + key.Name, reference.IsCollection, attrs);
            model.Properties.Add(propertyModel.Name, propertyModel);
        }

        private static IReadOnlyCollection<Attribute> AddAttribute( IEntityReferenceModel reference)
        {
            var attrs = reference.Attributes;
            attrs = attrs.Concat(new Attribute[]
            {
                new RelationAttribute(reference.TargetEntity),
                new IndexAttribute()
            }).ToArray();
            return attrs;
        }
    }
}