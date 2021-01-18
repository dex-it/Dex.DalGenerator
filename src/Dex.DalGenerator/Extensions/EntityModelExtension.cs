using System.Collections.Generic;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.Extensions;
using Dex.DalGenerator.Generator;
using Dex.DalGenerator.Annotation.Attributes;

namespace Dex.DalGenerator.Extensions
{
    public static class EntityModelExtension
    {
        public static Relation[] CollectRelations(this IEntityModel[] entities)
        {
            var result = new List<Relation>();
            foreach (var entity in entities)
            {
                foreach (var propertyModel in entity.Properties.Values)
                {
                    var relationAttribute =
                        (ReferencesAttribute) propertyModel.Attributes.FirstOrDefault(a => a is ReferencesAttribute);
                    if (relationAttribute == null) continue;

                    var otherModel = entities.First(m => m.SourceType == relationAttribute.Type);
                    var relation = new Relation
                    {
                        EntityName = entity.Name,
                        IsCollection = false,
                        KeyPropertyName = propertyModel.Name,
                        TypeName = otherModel.Name,
                        PropertyName = propertyModel.Name.ReplaceRegex("Id$", string.Empty),
                        IsBackRelation = false,
                        IsCascadeDelete = relationAttribute.Cascade
                    };
                    var backRelation = new Relation
                    {
                        EntityName = otherModel.Name,
                        IsCollection = true,
                        KeyPropertyName = propertyModel.Name,
                        TypeName = entity.Name,
                        PropertyName = entity.Name.ReplaceRegex("Db$", string.Empty),
                        IsBackRelation = true,
                        IsCascadeDelete = relationAttribute.Cascade
                    };
                    if (relationAttribute.OneToOne)
                    {
                        backRelation.IsCollection = false;
                        backRelation.PropertyName = backRelation.PropertyName;
                        backRelation.OneToOne = true;
                        relation.OneToOne = true;
                    }

                    result.Add(relation);
                    result.Add(backRelation);
                }
            }

            var duplicatedBackRelations = result
                .Where(r => result
                    .Where(i => i.EntityName == r.EntityName && i != r)
                    .Any(i => i.IsCollection && i.PropertyName == r.PropertyName));

            foreach (var duplicatedBackRelation in duplicatedBackRelations)
            {
                var keyName = duplicatedBackRelation.KeyPropertyName.ReplaceRegex("Id$", string.Empty);
                duplicatedBackRelation.PropertyName = keyName + duplicatedBackRelation.PropertyName;
            }

            return result.ToArray();
        }
    }
}