using System;
using System.Collections.Generic;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Common
{
    internal class CustomEntityModel : IEntityModel
    {
        public IEnumerable<Attribute> Attributes { get; } = Array.Empty<Attribute>();
        public string Name { get; set; }
        public Type SourceType { get; set; }
        public List<Type> Implements { get; set; } = new List<Type>();
        public bool IsRootType { get; set; }
        public Dictionary<string, IPropertyModel> Properties { get; } = new Dictionary<string, IPropertyModel>();
        public Dictionary<string, IEntityReferenceModel> References { get; } = new Dictionary<string, IEntityReferenceModel>();
    }
}