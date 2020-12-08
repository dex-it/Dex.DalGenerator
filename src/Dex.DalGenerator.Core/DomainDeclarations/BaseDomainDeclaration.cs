using System;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Extensions;
using Dex.DalGenerator.Annotation.Attributes;

namespace Dex.DalGenerator.Core.DomainDeclarations
{
    public abstract class BaseDomainDeclaration : IDomainDeclaration
    {
        public abstract Type[] GetTypes();

        protected static bool CheckHasIEntityInterface(Type type, bool isDb)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.IsInterface &&
                   (type.GetInterface("IEntity`1") != null ||
                    (type.GetInterface("IEntity") != null && !isDb));
        }

        protected static bool CheckIgnoreAttributes(Type type, bool isDb)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetAttributes<IgnoreAttribute>().IsNullOrEmpty() &&
                   (type.GetAttributes<IgnoreDbAttribute>().IsNullOrEmpty() || !isDb);
        }
    }
}