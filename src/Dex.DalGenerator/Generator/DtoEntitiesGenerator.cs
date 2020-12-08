using System;
using System.IO;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Core.Extensions;
using Dex.DalGenerator.Extensions;
using Dex.DalGenerator.Templates;
using DtoEntityGenerator = Dex.DalGenerator.Templates.DtoEntityGenerator;

namespace Dex.DalGenerator.Generator
{
    class DtoEntitiesGenerator
    {
        private readonly IEntityModel[] _entities;
        private readonly string _namespace;
        private readonly string _enumNamespace;


        public DtoEntitiesGenerator(IEntityModel[] entities, string @namespace, string enumNamespace)
        {
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
            _namespace = @namespace;
            _enumNamespace = enumNamespace;
        }

        public void Generate(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            Directory.GetFiles(folderPath, "*.g.cs").ForEach(File.Delete);

            foreach (var entityModel in _entities)
            {
                new DtoEntityGenerator(entityModel, _namespace, _enumNamespace, folderPath)
                    .WriteToFile(Path.Combine(folderPath, $"{entityModel.Name}.g.cs"));
            }
        }
    }
}