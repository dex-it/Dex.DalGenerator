using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Core.Extensions;
using Dex.Ef.Contracts.Entities;

namespace Dex.DalGenerator.Core.Generators
{
    public class DomainEntityModelGenerator
    {
        private readonly DomainModel _domainModel = new DomainModel();


        public IReadOnlyCollection<IEntityModel> GetEntityModels(IEnumerable<Type> rootTypes)
        {
            if (rootTypes == null) throw new ArgumentNullException(nameof(rootTypes));
            return rootTypes.Select(GetEntityModel).ToArray();
        }

        public EntityModel GetEntityModel(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!type.GetTypeInfo().IsInterface)
            {
                throw new InvalidOperationException($"{type.Name} is not an interface.");
            }

            var entities = _domainModel.Entities;
            if (entities.ContainsKey(type))
            {
                return entities[type];
            }

            var isRoot = type.GetTypeInfo().FindInterfaces(ObjectExtensions.RootInterfaceFilter, null).Any();

            var entityName = GetEntityName(type);
            var customAttributes = type.GetTypeInfo().GetCustomAttributes();
            var entityModel = new EntityModel(entityName, type, isRoot, customAttributes);
            entities.Add(type, entityModel);
            FillEntityModel(entityModel, type, true);
            return entityModel;
        }


        private void FillEntityModel(EntityModel entity, Type type, bool includeInhereted = false)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsInterface)
            {
                throw new InvalidOperationException();
            }

            if (includeInhereted)
            {
                var subInterfaces = typeInfo.GetInterfaces();
                foreach (var subInterface in subInterfaces)
                {
                    FillEntityModel(entity, subInterface);
                }
            }

            var props = typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in props)
            {
                var propertyType = property.PropertyType;
                var attrs = property.GetCustomAttributes().ToArray();
                if (propertyType.IsArray)
                {
                    var itemType = propertyType.GetElementType();
                    AddPropertyOrReference(entity, itemType, property, true, attrs);
                }
                else
                {
                    AddPropertyOrReference(entity, propertyType, property, false, attrs);
                }
            }
        }

        private void AddPropertyOrReference(EntityModel model, Type propType, PropertyInfo property, bool isCollection,
            IEnumerable<Attribute> attrs)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propType == null) throw new ArgumentNullException(nameof(propType));
            if (attrs == null) throw new ArgumentNullException(nameof(attrs));

            var name = property.Name;
            if (IsEntity(propType))
            {
                var target = GetEntityModel(propType);
                var entityReference = new EntityReferenceModel(target, name, property.DeclaringType, isCollection,
                    attrs.ToArray(), property.CanWrite);
                model.References.Add(name, entityReference);
            }
            else
            {
                var propertyModel = new PropertyModel(property, propType, name, isCollection, attrs.ToArray());
                model.Properties.Add(name, propertyModel);
            }
        }


        private static bool IsEntity(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().IsInterface && typeof(IEntity).GetTypeInfo().IsAssignableFrom(type);
        }

        private static string GetEntityName(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var typeName = type.Name.StartsWith("I") ? type.Name.Substring(1) : type.Name;
            return typeName;
        }
    }
}