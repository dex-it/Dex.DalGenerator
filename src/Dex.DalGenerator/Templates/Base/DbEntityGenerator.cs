using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.Extensions;
using Dex.DalGenerator.Generator;

// ReSharper disable once CheckNamespace
namespace Dex.DalGenerator.Templates
{
    public partial class DbEntityGenerator : IEntityGenerator
    {
        public Relation[] Relations { get; }
        public string Namespace { get; }
        public string EnumNamespace { get; }
        public IEntityModel Entity { get; }

        public DbEntityGenerator(IEntityModel model, Relation[] relations, string @namespace, string enumNamespace)
        {
            Relations = relations;
            Namespace = @namespace;
            EnumNamespace = enumNamespace;

            Entity = model ?? throw new ArgumentNullException(nameof(model));
            Entity.Properties.Values.First().PropertyType.GetFriendlyName();
        }

        public string Generate()
        {
            return TransformText();
        }

        private string GetTableName(IEntityModel model)
        {
            return model.GetTableName();
        }

        private string GetAttributes(IPropertyModel property)
        {
            const string newLine = "\n\t\t";
            var attributeResult = string.Empty;
            if (property.IsKey())
            {
                attributeResult += "[Key]" + newLine;
            }

            if (property.IsIdentity())
            {
                attributeResult += "[Identity]" + newLine;
            }

            foreach (var attribute in property.Attributes)
            {
                if (attribute is MaxLengthAttribute maxLengthAttribute)
                {
                    attributeResult += $"[MaxLength({maxLengthAttribute.Length})]" + newLine;
                }
                else if (attribute is MinLengthAttribute minLengthAttribute)
                {
                    attributeResult += $"[MinLength({minLengthAttribute.Length})]" + newLine;
                }
                else if (attribute is StringLengthAttribute stringLengthAttribute)
                {
                    var max = stringLengthAttribute.MaximumLength;
                    var min = stringLengthAttribute.MinimumLength;
                    attributeResult += $"[StringLength({max}, MinimumLength = {min})]" + newLine;
                }
                else if (attribute is RequiredAttribute)
                {
                    attributeResult += "[Required]" + newLine;
                }
            }

            attributeResult += $@"[Column(""{property.Name}"")]";
            return attributeResult;
        }
    }
}