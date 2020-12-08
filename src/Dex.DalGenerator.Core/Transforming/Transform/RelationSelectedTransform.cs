using System;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public abstract class RelationSelectedTransform : RelationTransform
    {
        private readonly Func<IEntityReferenceModel, bool> _selector;

        protected RelationSelectedTransform(Func<IEntityReferenceModel, bool> selector)
        {
            _selector = selector;
        }

        public override bool Selector(IEntityReferenceModel reference)
        {
            return _selector(reference);
        }
    }
}