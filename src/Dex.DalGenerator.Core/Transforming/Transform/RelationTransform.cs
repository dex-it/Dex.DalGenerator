
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public abstract class RelationTransform
    {
        public abstract bool Selector(IEntityReferenceModel reference);
        public abstract void Transform(IEntityModel model, IEntityReferenceModel reference);
    }
}