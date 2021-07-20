using System;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class SetPropertyReadOnlyTransform : PropertyTransform
	{
		private readonly Func<IPropertyModel, bool> _selector;

		public SetPropertyReadOnlyTransform(Func<IPropertyModel, bool> selector)
		{
			_selector = selector;
		}

		public override bool Selector(IPropertyModel property)
		{
			return _selector(property);
		}

		public override void Transform(IEntityModel model, IPropertyModel property)
		{
		    if (model == null) throw new ArgumentNullException(nameof(model));
		    if (property == null) throw new ArgumentNullException(nameof(property));

		    var newProp = new PropertyModel(property.MemberInfo, property.PropertyType, property.Name, property.IsCollection, property.Attributes, false);
			model.Properties.Add(property.Name, newProp);
		}
	}
}