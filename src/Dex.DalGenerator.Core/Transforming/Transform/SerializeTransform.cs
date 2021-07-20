using System;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Annotation.Attributes;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class SerializeTransform : PropertyTransform
    {
        private readonly Func<IPropertyModel, bool> _selector;

        public SerializeTransform(Func<IPropertyModel, bool> selector)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        public override bool Selector(IPropertyModel property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return _selector(property);
        }

        public override void Transform(IEntityModel model, IPropertyModel property)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (property == null) throw new ArgumentNullException(nameof(property));

            var attributes = property.Attributes.Concat(new[] { new SerializeIntoStringFieldAttribute() }).ToArray();
            var propertyModel = new PropertyModel(property.MemberInfo, typeof(string), property.Name, isCollection: false, attributes);

            model.Properties.Add(property.Name, propertyModel);
        }
    }
}