using System;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;

namespace Dex.DalGenerator.Extensions
{
    public static class AttributesExtensions
    {
        public static T GetAttribute<T>(this IHasAttributes item) where T : Attribute
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            return item.Attributes?.OfType<T>().FirstOrDefault();
        }
    }
}