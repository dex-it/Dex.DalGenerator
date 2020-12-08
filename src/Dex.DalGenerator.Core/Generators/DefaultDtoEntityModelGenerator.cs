using System;
using System.Linq;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Generators
{
    public class DefaultDtoEntityModelGenerator : IDomainEntityModelGenerator
    {
        private readonly IDomainDeclaration _domainDeclaration;

        public DefaultDtoEntityModelGenerator(IDomainDeclaration domainDeclaration)
        {
            if (domainDeclaration == null) throw new ArgumentNullException(nameof(domainDeclaration));
            _domainDeclaration = domainDeclaration;
        }

        public IEntityModel[] GetModels()
        {
            var types = _domainDeclaration.GetTypes();
            return new DomainEntityModelGenerator().GetEntityModels(types).ToArray();
        }
    }
}