using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.EntityModels;
using Dex.DalGenerator.Core.Extensions;

namespace Dex.DalGenerator.Core.Transforming.Transform
{
    public class SerializeIdsTransform : RelationTransform
    {
        public override bool Selector(IEntityReferenceModel reference)
        {
            return reference.IsCollection && reference.TargetEntity.SourceType.IsSimple();
        }

        public override void Transform(IEntityModel model, IEntityReferenceModel reference)
        {
            var name = reference.Name;
            var propertyModel = new PropertyModel(typeof(string), reference.Name, false, reference.Attributes);
            model.Properties.Add(name, propertyModel);
        }
    }
}