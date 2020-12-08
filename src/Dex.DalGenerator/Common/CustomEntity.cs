using System;
using System.Collections.Generic;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Common
{
    internal class CustomPropertyModel: IPropertyModel
    {
        public string PropertyTypeName { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public Type PropertyType { get; set; }
        public string Name { get; set; }
        public bool IsCollection { get; set; }
        public bool CanWrite { get; set; }
        public bool IsKey { get; set; }
        public bool IsAutoIncrement { get; set; }
    }

    internal class CustomEntityModel: IEntityModel
    {
        public IEnumerable<Attribute> Attributes { get; } = new Attribute[0];
        public string Name { get; set; }
        public Type SourceType { get; set; }
        public List<Type> Implements { get; set; } = new List<Type>();
        public bool IsRootType { get; set; }
        public Dictionary<string, IPropertyModel> Properties { get; } = new Dictionary<string, IPropertyModel>();
        public Dictionary<string, IEntityReferenceModel> References { get; } = new Dictionary<string, IEntityReferenceModel>();
    }
}