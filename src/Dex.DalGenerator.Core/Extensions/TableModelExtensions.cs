using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class TableModelExtensions
    {
        public static string GetTableName(this IEntityModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var table = entity.Attributes.OfType<TableAttribute>().FirstOrDefault();
            if (table == null) throw new NullReferenceException("table");
            return table.Name;
        }

        public static string IsRootType(this IEntityModel entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var table = entity.Attributes.OfType<TableAttribute>().FirstOrDefault();
            if (table == null) throw new NullReferenceException("table");
            return table.Name;
        }
    }
}