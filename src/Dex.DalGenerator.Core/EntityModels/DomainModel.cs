using System;
using System.Collections.Generic;

namespace Dex.DalGenerator.Core.EntityModels
{
    public class DomainModel
    {
        public DomainModel(Dictionary<Type, EntityModel> entities = null)
        {
            Entities = entities ?? new Dictionary<Type, EntityModel>();
        }

        public Dictionary<Type, EntityModel> Entities { get; private set; }
    }
}