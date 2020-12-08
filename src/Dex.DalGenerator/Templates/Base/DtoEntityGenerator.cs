using System;
using Dex.DalGenerator.Common;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.Extensions;

// ReSharper disable once CheckNamespace
namespace Dex.DalGenerator.Templates
{
    public partial class DtoEntityGenerator : IEntityGenerator
    {
        private readonly string _customSourceTypeName;
        public IEntityModel Entity { get; }
        private readonly string _namespace;
        private readonly string _enumNamespace;

        public DtoEntityGenerator(IEntityModel model, string @namespace, string enumNamespace,
            string customSourceTypeName = null)
        {
            _customSourceTypeName = customSourceTypeName;
            _namespace = @namespace;
            _enumNamespace = enumNamespace;
            Entity = model ?? throw new ArgumentNullException(nameof(model));
        }

        private string GetFriendlyName(IPropertyModel propertyModel)
        {
            if (propertyModel is CustomPropertyModel customProperty)
            {
                return customProperty.PropertyTypeName;
            }
            else
            {
                return propertyModel.PropertyType.GetFriendlyName();
            }
        }

        private string GetSourceTypeName()
        {
            return _customSourceTypeName ?? Entity.SourceType.ToString();
        }

        public string Generate()
        {
            return TransformText();
        }
    }
}