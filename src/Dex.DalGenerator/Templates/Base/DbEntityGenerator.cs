using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
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
        public string[] EnumNamespaces { get; }
        public bool IsSnakeCase { get; }
        public bool IsInternal { get; }
        public IEntityModel Entity { get; }

        public DbEntityGenerator(IEntityModel model, Relation[] relations, string @namespace, string[] enumNamespaces,
            bool isSnakeCase, bool isInternal)
        {
            Relations = relations;
            Namespace = @namespace;
            EnumNamespaces = enumNamespaces;
            IsSnakeCase = isSnakeCase;
            IsInternal = isInternal;

            Entity = model ?? throw new ArgumentNullException(nameof(model));
            Entity.Properties.Values.First().PropertyType.GetFriendlyName();
        }

        public string Generate()
        {
            return TransformText();
        }

        private string GetTableName(IEntityModel model)
        {
            var tableName = model.GetTableName();
            return IsSnakeCase ? tableName.ToSnakeCase() : tableName;
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

            var columnName = IsSnakeCase ? property.Name.ToSnakeCase() : property.Name;
            attributeResult += $@"[Column(""{columnName}"")]";
            return attributeResult;
        }
    }
}