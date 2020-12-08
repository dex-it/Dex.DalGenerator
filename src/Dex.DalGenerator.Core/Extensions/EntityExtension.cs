using System;
using System.IO;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.Ef.Contracts.Entities;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class EntityExtension
    {
        public static void CheckKey<TK>( this IEntity<TK> item)
            where TK : IComparable
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (Equals(item.Id, default(TK)))
            {
                throw new InvalidDataException("key value can't be empty");
            }
        }

        public static IPropertyModel GetKey( this IEntityModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            return model.Properties.Select(pair => pair.Value).FirstOrDefault(propertyModel => propertyModel.IsKey);
        }

    }
}