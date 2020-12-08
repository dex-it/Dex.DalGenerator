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

        public SerializeTransform( Func<IPropertyModel, bool> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            _selector = selector;
        }

        public override bool Selector( IPropertyModel property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return _selector(property);
        }

        public override void Transform( IEntityModel model,  IPropertyModel property)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (property == null) throw new ArgumentNullException(nameof(property));

            var name = property.Name;

            var attributes = property.Attributes.Concat(new[] { new SerializeIntoStringFieldAttribute() }).ToArray();
            var propertyModel = new PropertyModel(typeof(string), name, false, attributes);

            model.Properties.Add(name, propertyModel);
        }
    }
}