using System;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core
{
    public abstract class BaseEntityDomain : IEntityDomain
    {
        public IEntityModel[] EntityModels { get; }

        protected BaseEntityDomain(IDomainEntityModelGenerator entityModelGenerator)
        {
            if (entityModelGenerator == null) throw new ArgumentNullException(nameof(entityModelGenerator));
            EntityModels = entityModelGenerator.GetModels();
        }
    }
}