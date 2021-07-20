using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Annotation.Attributes;
using System.Reflection;
using System.Globalization;

namespace Dex.DalGenerator.Core.EntityModels
{
    public class PropertyModel : IPropertyModel
    {
        public const int MaxLengthFieldName = 63;

        public PropertyModel(MemberInfo? memberInfo,
            Type propertyType, 
            string name,
            bool isCollection = false, 
            IEnumerable<Attribute>? attributes = null, bool canWrite = true)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Length > MaxLengthFieldName)
            {
                throw new ArgumentNullException(nameof(name), string.Format(CultureInfo.InvariantCulture, 
                    "ProperyName: {2}, Length: {0} > MaxLength: {1}", name.Length, MaxLengthFieldName, name));
            }

            IsCollection = isCollection;
            Name = name;
            Attributes = attributes ?? Array.Empty<Attribute>();
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            CanWrite = canWrite;
            MemberInfo = memberInfo;
        }

        public MemberInfo? MemberInfo { get; }

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