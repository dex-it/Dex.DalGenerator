using System;
using System.Collections.Generic;

namespace Dex.DalGenerator.Core.Contracts.EntityModel
{
    public interface IEntityReferenceModel
    {
        IEntityModel TargetEntity { get; }
        string Name { get; }
        Type DeclaringType { get; }
        bool IsCollection { get; }
        IReadOnlyCollection<Attribute> Attributes { get; }
        bool CanWrite { get; }
        bool HasAttribute<T>() where T : Attribute;
    }
}