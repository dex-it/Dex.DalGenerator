using System;
using System.Collections.Generic;

namespace Dex.DalGenerator.Core.Contracts.EntityModel
{
    public interface IEntityModel : IHasAttributes
    {
        string Name { get; }
        Type SourceType { get; set; }
        List<Type> Implements { get; set; }
        bool IsRootType { get; set; }
        Dictionary<string, IPropertyModel> Properties { get; }
        Dictionary<string, IEntityReferenceModel> References { get; }
    }
}