using System;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class AttributesExtensions
    {
        public static bool HasAttribute<T>( this IHasAttributes item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (item.Attributes == null) return false;
            return item.Attributes.OfType<T>().Any();
        }
    }
}