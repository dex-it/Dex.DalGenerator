﻿using System;

namespace Dex.DalGenerator.Core.Contracts.EntityModel
{
    public interface IPropertyModel : IHasAttributes
    {
        Type PropertyType { get; }
        string Name { get; }
        bool IsCollection { get; }
        bool CanWrite { get; }
        bool IsKey { get; }
        bool IsAutoIncrement { get; }
    }
}