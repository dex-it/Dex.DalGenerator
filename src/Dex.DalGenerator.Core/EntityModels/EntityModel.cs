using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.EntityModels
{
    public class EntityModel : IEntityModel
    {
        public EntityModel(string name, Type sourceType, bool isRootType, IEnumerable<Attribute> attributes)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            Name = name ?? throw new ArgumentNullException(nameof(name));
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            IsRootType = isRootType;
            Properties = new Dictionary<string, IPropertyModel>();
            References = new Dictionary<string, IEntityReferenceModel>();
            Implements = new List<Type>();
            Attributes = new List<Attribute>(attributes);
        }

        public string Name { get; }

        public Type SourceType { get; set; }

        public List<Type> Implements { get; set; }

        public bool IsRootType { get; set; }

        public IEnumerable<Attribute> Attributes { get; private set; }

        public Dictionary<string, IPropertyModel> Properties { get; }

        public Dictionary<string, IEntityReferenceModel> References { get; }

        public void AddAttribute(TableAttribute tableAttribute)
        {
            if (tableAttribute == null) throw new ArgumentNullException(nameof(tableAttribute));
            Attributes = new List<Attribute>(Attributes) {tableAttribute};
        }

        public override string ToString()
        {
            return "Entity: " + Name;
        }
    }
}