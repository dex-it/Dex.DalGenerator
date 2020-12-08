using System;

namespace Dex.DalGenerator.Core.Contracts
{
    public interface IDomainDeclaration
    {
        Type[] GetTypes();
    }
}