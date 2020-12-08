using System;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Contracts.Attributes
{
    public class RelationAttribute : Attribute
    {
        public RelationAttribute(IEntityModel model, bool cascade = false)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Cascade = cascade;
        }

        public IEntityModel Model { get; }

        public bool Cascade { get; }
    }
}