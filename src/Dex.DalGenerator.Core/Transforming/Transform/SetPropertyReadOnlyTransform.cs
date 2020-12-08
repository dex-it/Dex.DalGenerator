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

		public override void Transform( IEntityModel model,  IPropertyModel prop)
		{
		    if (model == null) throw new ArgumentNullException(nameof(model));
		    if (prop == null) throw new ArgumentNullException(nameof(prop));

		    var newProp = new PropertyModel(prop.PropertyType, prop.Name, prop.IsCollection, prop.Attributes, false);
			model.Properties.Add(prop.Name, newProp);
		}
	}
}