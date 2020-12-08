using System;
using System.Linq;
using System.Reflection;

namespace Dex.DalGenerator.Core.DomainDeclarations
{
    public class DefaultDbDomainDeclaration : BaseDomainDeclaration
    {
        private Assembly DomainAssembly { get; }

        public DefaultDbDomainDeclaration(Assembly domainAssembly)
        {
            if (domainAssembly == null) throw new ArgumentNullException(nameof(domainAssembly));
            DomainAssembly = domainAssembly;
        }

        public override Type[] GetTypes()
        {
            return DomainAssembly.GetTypes()
                .Where(type => CheckHasIEntityInterface(type, true))
                .Where(type => CheckIgnoreAttributes(type, true))
                .ToArray();
        }
    }
}