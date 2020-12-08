using System;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class ChangeTypeTransform : PropertyTransform
    {
        private readonly Func<IPropertyModel, bool> _selector;
        private readonly Type _newType;

        public ChangeTypeTransform(Func<IPropertyModel, bool> selector, Type newType)
        {
            _selector = selector;
            _newType = newType;
        }

        public override bool Selector(IPropertyModel property)
        {
            return _selector(property);
        }

        public override void Transform(IEntityModel model, IPropertyModel property)
        {
            var name = property.Name;
            var propertyModel = new PropertyModel(_newType, name, false, property.Attributes);
            model.Properties.Add(name, propertyModel);
        }
    }
}