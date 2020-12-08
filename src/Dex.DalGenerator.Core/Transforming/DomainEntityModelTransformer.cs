using System;
using System.Collections.Generic;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Core.Extensions;

namespace Dex.DalGenerator.Core.Transforming
{
    public class DomainEntityModelTransformer
    {
        private readonly DomainTransformerConfig _domainTransformerConfig;
        private readonly Dictionary<IEntityModel, IEntityModel> _transformationCache = new Dictionary<IEntityModel, IEntityModel>();

        public DomainEntityModelTransformer(DomainTransformerConfig domainTransformerConfig)
        {
            if (domainTransformerConfig == null) throw new ArgumentNullException(nameof(domainTransformerConfig));

            _domainTransformerConfig = domainTransformerConfig;
        }

        public IEntityModel Transform(IEntityModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if (_transformationCache.ContainsKey(model))
            {
                var entityModel = _transformationCache[model];
                return entityModel;
            }

            var generatorRules = _domainTransformerConfig.Transformations;
            if (generatorRules != null)
            {
                var resultModel = new EntityModel(model.Name, model.SourceType, model.IsRootType, model.Attributes)
                {
                    Implements = model.Implements
                };
                _transformationCache.Add(model, resultModel);
                
                // entity transform
                var entityRules = generatorRules.Where(rule => rule.EntitySelector(model)).ToArray();
                entityRules
                    .Where(transformation => transformation.EntityTransform != null)
                    .ForEach(transformation =>
                    {
                        transformation.EntityTransform(resultModel);
                    });

                // property
                var propertyRules = entityRules
                    .Where(transformation => transformation.PropTransformations != null)
                    .SelectMany(transformation => transformation.PropTransformations)
                    .ToArray();

                foreach (var propertyModel in model.Properties.Values)
                {
                    var rulesForModel = propertyRules
                        .Where(rule => rule.Selector(propertyModel))
                        .ToArray();

                    if (!rulesForModel.Any())
                    {
                        resultModel.Properties.Add(propertyModel.Name, propertyModel);
                    }

                    foreach (var propRule in rulesForModel)
                    {
                        propRule.Transform(resultModel, propertyModel);
                    }
                }

                // relation
                var relationRules = entityRules
                    .Where(transformation => transformation.RelTransforms != null)
                    .SelectMany(rule => rule.RelTransforms)
                    .ToArray();

                foreach (var reference in model.References.Values)
                {
                    var transformedTarget = Transform(reference.TargetEntity);
                    var refToTransformedEntity = new EntityReferenceModel(transformedTarget, reference.Name, reference.DeclaringType, reference.IsCollection, reference.Attributes, reference.CanWrite);

                    var refRules = relationRules
                        .Where(rule => rule.Selector(reference))
                        .ToArray();

                    var refRule = refRules.FirstOrDefault(transform => transform.Selector(reference));
                    if (refRule != null)
                    {
                        refRule.Transform(resultModel, refToTransformedEntity);
                    }
                    else
                    {
                        resultModel.References.Add(reference.Name, refToTransformedEntity);
                    }
                }

                // add transform
                var toAdd = entityRules
                    .Where(transformation => transformation.AddProps != null)
                    .SelectMany(transformation => transformation.AddProps)
                    .ToArray();

                foreach (var propertyModel in toAdd)
                {
                    resultModel.Properties.Add(propertyModel.Name, propertyModel);
                }

                return resultModel;
            }

            _transformationCache.Add(model, model);
            return model;
        }
    }
}