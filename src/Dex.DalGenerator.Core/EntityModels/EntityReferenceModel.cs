using System;
using System.Collections.Generic;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.EntityModels
{

    public class EntityReferenceModel : IEntityReferenceModel
    {
        public EntityReferenceModel( IEntityModel targetEntity, string name, Type declaringType,
            bool isCollection, IReadOnlyCollection<Attribute> attributes, bool canWrite)
        {
            if (declaringType == null) throw new ArgumentNullException(nameof(declaringType));

            IsCollection = isCollection;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TargetEntity = targetEntity ?? throw new ArgumentNullException(nameof(targetEntity));
            Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
            CanWrite = canWrite;
            DeclaringType = declaringType;
        }

        
        public IEntityModel TargetEntity { get; private set; }

        
        public string Name { get; private set; }

        
        public Type DeclaringType { get; private set; }

	    public bool IsCollection { get; private set; }

        
        public IReadOnlyCollection<Attribute> Attributes { get; private set; }

        public bool CanWrite { get; private set; }

	    public override string ToString()
        {
            return Name;
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return Attributes.OfType<T>().Any();
        }
    }
}