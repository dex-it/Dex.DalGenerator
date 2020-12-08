using System;
using System.Linq;
using System.Reflection;

namespace Dex.DalGenerator.Core.DomainDeclarations
{
    public class DefaultDtoDomainDeclaration : BaseDomainDeclaration
    {
        private Assembly DomainAssembly { get; }

        public DefaultDtoDomainDeclaration(Assembly domainAssembly)
        {
            if (domainAssembly == null) throw new ArgumentNullException(nameof(domainAssembly));
            DomainAssembly = domainAssembly;
        }

        public override Type[] GetTypes()
        {
            return DomainAssembly.GetTypes()
                .Where(type => CheckHasIEntityInterface(type, false))
                .Where(type => CheckIgnoreAttributes(type, false))
                .ToArray();
        }
    }
}