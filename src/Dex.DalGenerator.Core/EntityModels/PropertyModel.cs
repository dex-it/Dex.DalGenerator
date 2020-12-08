using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Annotation.Attributes;

#if !NET45
#endif

namespace Dex.DalGenerator.Core.EntityModels
{
    public class PropertyModel : IPropertyModel
    {
        public const int MaxLengthFieldName = 63;

        public PropertyModel( Type propertyType,  string name,
            bool isCollection = false, IEnumerable<Attribute> attributes = null, bool canWrite = true)
        {
            if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Length > MaxLengthFieldName)
            {
                throw new ArgumentNullException(nameof(name), string.Format("ProperyName: {2}, Length: {0} > MaxLength: {1}", name.Length, MaxLengthFieldName, name));
            }


            IsCollection = isCollection;
            Name = name;
            Attributes = attributes ?? new Attribute[] { };
            PropertyType = propertyType;
            CanWrite = canWrite;
        }

        
        public Type PropertyType { get; }

        
        public string Name { get; }

        
        public IEnumerable<Attribute> Attributes { get; }

        public bool IsCollection { get; }

        public bool CanWrite { get; }

#if !NET45
        public bool IsKey
        {
            get { return Attributes.OfType<KeyAttribute>().Any(); }
        }
#endif

        public bool IsAutoIncrement
        {
            get { return Attributes.OfType<AutoIncrementAttribute>().Any(); }
        }

        public override string ToString()
        {
            return $"{PropertyType.Name} {Name}";
        }
    }
}