using System;
using System.Reflection;
using Dex.Ef.Contracts.Entities;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool RootInterfaceFilter( Type typeObj,  Object criteriaObj)
        {
            if (typeObj == null) throw new ArgumentNullException(nameof(typeObj));
            return typeObj.GetTypeInfo().IsGenericType && typeObj.GetGenericTypeDefinition() == typeof(IEntity<>);
        }
    }
}