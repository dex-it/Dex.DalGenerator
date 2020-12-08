using System;
using System.Collections.Generic;
using System.Linq;
using Dex.DalGenerator.Core.Contracts;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.Extensions;
using Dex.DalGenerator.Core.Transforming;
using Dex.DalGenerator.Core.Transforming.Transform;
using Dex.Ef.Contracts.Entities;

namespace Dex.DalGenerator.Core.Generators
{
    public class DefaultDbEntityModelGenerator : IDomainEntityModelGenerator
    {
        private readonly IDomainDeclaration _domainDeclaration;

        public DefaultDbEntityModelGenerator(IDomainDeclaration domainDeclaration)
        {
            if (domainDeclaration == null) throw new ArgumentNullException(nameof(domainDeclaration));
            _domainDeclaration = domainDeclaration;
        }

        public IEntityModel[] GetModels()
        {
            var types = _domainDeclaration.GetTypes();
            var entityModels = new DomainEntityModelGenerator().GetEntityModels(types);

            var step0 = PropsTransforms(entityModels);
            var step1 = SerializeTransform(step0);
            var step2 = ReferenceTransform(step1);
            var step3 = FlatTransform(step2);
            var result = step3;

            var conf = new TableTransformerConfig
            {
                EntityNameProvider = model => model.Name + "Db",
                TableNameProvider = model => model.Name.Pluralize()
            };
            var tt = new EntityTableTransformer(conf);
            var tables = result.Select(x => tt.Transform(x)).ToArray();

            return tables;
        }

        protected virtual IEntityModel[] PropsTransforms(IReadOnlyCollection<IEntityModel> entityModels)
        {
            var dtoGeneratorConfig = new DomainTransformerConfig
            {
                Transformations = new[]
                {
                    new EntityTransformationContext(model =>
                        model.SourceType.GetInterfaces().Any(type => typeof(IDeletable).IsAssignableFrom(type)))
                    {
                        EntityTransform = model => { model.Implements.Add(typeof(IDeletable)); }
                    },
                    new EntityTransformationContext(model =>
                        model.SourceType.GetInterfaces().Any(type => typeof(ICreatedUtc).IsAssignableFrom(type)))
                    {
                        EntityTransform = model => { model.Implements.Add(typeof(ICreatedUtc)); }
                    },
                    new EntityTransformationContext(model =>
                        model.SourceType.GetInterfaces().Any(type => typeof(IUpdatedUtc).IsAssignableFrom(type)))
                    {
                        EntityTransform = model => { model.Implements.Add(typeof(IUpdatedUtc)); }
                    },
                    new EntityTransformationContext(model => true)
                    {
                        EntityTransform = model =>
                        {
                            var item = model.SourceType.FindInterfaces(TypeExtensions.RootInterfaceFilter, null)
                                .FirstOrDefault();
                            if (item != null)
                            {
                                model.Implements.Add(item);
                                model.Implements.Add(typeof(IDbEntity));
                            }
                        }
                    },
                }
            };

            var dbModels = Transform(entityModels, dtoGeneratorConfig);
            return dbModels;
        }

        protected virtual IEntityModel[] SerializeTransform(IReadOnlyCollection<IEntityModel> entityModels)
        {
            var dtoGeneratorConfig = new DomainTransformerConfig
            {
                Transformations = new[]
                {
                    new EntityTransformationContext(model => true)
                    {
                        PropTransformations = new List<PropertyTransform>
                        {
                            new SetPropertyReadOnlyTransform(model =>
                                model.IsCollection && model.PropertyType.FullName != null
                                                   && model.PropertyType.FullName.StartsWith("System")),
                            new SerializeTransform(model => model.IsCollection
                                                            && (model.PropertyType.FullName == null
                                                                || !model.PropertyType.FullName.StartsWith("System"))),
                            new ChangeTypeTransform(model => model.PropertyType == typeof(Type), typeof(string))
                        },
                        RelTransforms = new List<RelationTransform>
                        {
                            new SerializeObjTransform(model => model.IsCollection && !model.TargetEntity.IsRootType)
                        }
                    },
                    new EntityTransformationContext(model => !model.IsRootType)
                    {
                        RelTransforms = new List<RelationTransform>
                        {
                            new SerializeIdsTransform()
                        }
                    },
                }
            };

            var dbModels = Transform(entityModels, dtoGeneratorConfig);
            return dbModels;
        }

        protected virtual IEntityModel[] ReferenceTransform(IReadOnlyCollection<IEntityModel> entityModels)
        {
            var config = new DomainTransformerConfig()
            {
                Transformations = new[]
                {
                    new EntityTransformationContext(model => model.IsRootType)
                    {
                        RelTransforms = new List<RelationTransform>
                        {
                            new ReferenceByIdTransform(reference =>
                                reference.TargetEntity.IsRootType && !reference.IsCollection),
                            new BackReferenceByIdTransform(reference => reference.IsCollection),
                        }
                    },
                    new EntityTransformationContext(model => !model.IsRootType)
                    {
                        RelTransforms = new List<RelationTransform>
                        {
                            new ReferenceByIdTransform(reference => reference.TargetEntity.IsRootType),
                            new SerializeIdsTransform()
                        }
                    },
                }
            };

            var dbModels = Transform(entityModels, config);
            return dbModels;
        }

        protected virtual IEntityModel[] FlatTransform(IReadOnlyCollection<IEntityModel> entityModels)
        {
            var dtoGeneratorConfig = new DomainTransformerConfig
            {
                Transformations = new[]
                {
                    new EntityTransformationContext(model => true) // for all
                    {
                        RelTransforms = new List<RelationTransform>
                        {
                            new IncludeTransform(reference =>
                                !reference.TargetEntity.IsRootType && !reference.IsCollection),
                        }
                    }
                }
            };

            var dbModels = Transform(entityModels, dtoGeneratorConfig);
            return dbModels;
        }

        protected IEntityModel[] Transform(IReadOnlyCollection<IEntityModel> entityModels,
            DomainTransformerConfig dtoGeneratorConfig)
        {
            var transformer = new DomainEntityModelTransformer(dtoGeneratorConfig);
            return entityModels.Select(transformer.Transform).ToArray();
        }
    }
}