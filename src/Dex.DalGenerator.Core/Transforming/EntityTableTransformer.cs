using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Core.Extensions;
using Dex.DalGenerator.Annotation.Attributes;

namespace Dex.DalGenerator.Core.Transforming
{
    public class EntityTableTransformer
    {
        private readonly TableTransformerConfig _config;

        public EntityTableTransformer(TableTransformerConfig config = null)
        {
            _config = config ?? new TableTransformerConfig();
        }

        public IEntityModel Transform(IEntityModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entityName = GetEntityName(entity);
            var tableModel = new EntityModel(entityName, entity.SourceType, entity.IsRootType, entity.Attributes);
            var tableName = GetTableName(entity);
            tableModel.AddAttribute(new TableAttribute(tableName));

            entity.Implements.ForEach(tableModel.Implements.Add);

            var properties = GetDbProperties(entity).ToList();
            properties.ForEach(pair => pair.Value.ForEach(model => tableModel.Properties.Add(model.Name, model)));

            return tableModel;
        }


        private IEnumerable<KeyValuePair<IPropertyModel, IPropertyModel[]>> GetDbProperties(IEntityModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var props = model.Properties.Values;
            foreach (var propertyModel in props.Where(x => x.Attributes.OfType<IgnoreDbAttribute>().IsNullOrEmpty()))
            {
                var dbProps = GetPropertyModels(propertyModel).ToArray();
                yield return new KeyValuePair<IPropertyModel, IPropertyModel[]>(propertyModel, dbProps);
            }
        }

        private IEnumerable<IPropertyModel> GetPropertyModels(IPropertyModel property)
        {
            var propName = property.Name;
            var propertyType = property.PropertyType;

            if (propName == "Id")
            {
                var attributes = property.Attributes.Concat(new Attribute[] {new KeyAttribute()}).ToList();
                var propertyModel = new PropertyModel(propertyType, propName, property.IsCollection, attributes);
                yield return propertyModel;
            }
            else
            {
                var propertyModel =
                    new PropertyModel(propertyType, propName, property.IsCollection, property.Attributes);
                yield return propertyModel;
            }
        }

        private string GetTableName(IEntityModel model)
        {
            var nameProvider = _config.TableNameProvider;
            if (nameProvider != null)
            {
                return nameProvider(model);
            }

            return model.Name.ToLowerInvariant();
        }

        private string GetEntityName( IEntityModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            var nameProvider = _config.EntityNameProvider;
            if (nameProvider != null)
            {
                return nameProvider(model);
            }

            return model.Name;
        }
    }

    public class TableTransformerConfig
    {
        public Func<IEntityModel, string> TableNameProvider { get; set; }
        public Func<IEntityModel, string> EntityNameProvider { get; set; }
    }
}