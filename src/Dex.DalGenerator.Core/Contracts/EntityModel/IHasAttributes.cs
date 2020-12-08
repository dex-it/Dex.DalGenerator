using System;
using System.Collections.Generic;

namespace Dex.DalGenerator.Core.Contracts.EntityModel
{
    public interface IHasAttributes
    {
        IEnumerable<Attribute> Attributes { get; }
    }
}