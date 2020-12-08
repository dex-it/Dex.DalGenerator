using System.Reflection;
using Dex.DalGenerator.Core.DomainDeclarations;
using Dex.DalGenerator.Core.Generators;

namespace Dex.DalGenerator.Core
{
    public class DefaultDtoEntityDomain : BaseEntityDomain
    {
        public DefaultDtoEntityDomain(Assembly assembly)
            : base(new DefaultDtoEntityModelGenerator(new DefaultDtoDomainDeclaration(assembly)))
        {
        }
    }
}