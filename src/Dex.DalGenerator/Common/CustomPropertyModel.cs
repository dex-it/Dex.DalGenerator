using System;
using System.Collections.Generic;
using System.Reflection;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Common
{
    internal class CustomPropertyModel : IPropertyModel
    {
        public CustomPropertyModel(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));
        }

        public MemberInfo MemberInfo { get; }
        public string PropertyTypeName { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public Type PropertyType { get; set; }
        public string Name { get; set; }
        public bool IsCollection { get; set; }
        public bool CanWrite { get; set; }
        public bool IsKey { get; set; }
        public bool IsAutoIncrement { get; set; }
    }
}