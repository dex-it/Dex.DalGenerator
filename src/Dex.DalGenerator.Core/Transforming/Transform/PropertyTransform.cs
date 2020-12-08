
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public abstract class PropertyTransform
    {
        public abstract bool Selector(IPropertyModel property);
        public abstract void Transform(IEntityModel model, IPropertyModel property);
    }
}