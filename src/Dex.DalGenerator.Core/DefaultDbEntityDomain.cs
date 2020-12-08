using System.Reflection;
using Dex.DalGenerator.Core.DomainDeclarations;
using Dex.DalGenerator.Core.Generators;

namespace Dex.DalGenerator.Core
{
    public class DefaultDbEntityDomain : BaseEntityDomain
    {
        public DefaultDbEntityDomain(Assembly assembly)
            : base(new DefaultDbEntityModelGenerator(new DefaultDbDomainDeclaration(assembly)))
        {
        }
    }
}