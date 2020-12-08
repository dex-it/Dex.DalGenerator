using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Contracts
{
    public interface IDomainEntityModelGenerator
    {
        IEntityModel[] GetModels();
    }
}
