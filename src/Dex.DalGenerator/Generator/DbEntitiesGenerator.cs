using System;
using System.IO;
using System.Linq;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Extensions;
using Dex.DalGenerator.Templates;
using DbEntityGenerator = Dex.DalGenerator.Templates.DbEntityGenerator;

namespace Dex.DalGenerator.Generator
{
    public class DbEntitiesGenerator
    {
        private readonly IEntityModel[] _entities;

        private readonly Relation[] _relations;
        
        private readonly string _ns;
        
        private readonly string[] _enumNamespaces;
        private readonly bool _isSnakeCase;

        public DbEntitiesGenerator(
            IEntityModel[] entities, 
            Relation[] relations,
            string modelNamespace,
            string[] enumNamespaces, 
            bool isSnakeCase)
        {
            _relations = relations ?? throw new ArgumentNullException(nameof(relations));
            _ns = modelNamespace;
            _enumNamespaces = enumNamespaces;
            _isSnakeCase = isSnakeCase;
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        public void Generate(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.g.cs")
                .Select(f => new
                {
                    FilePath = f,
                    Name = Path.GetFileName(f).Replace(".g.cs", string.Empty)
                }).ToArray();

            foreach (var entityModel in _entities)
            {
                var relations = _relations.Where(r => r.EntityName == entityModel.Name).ToArray();
                new DbEntityGenerator(entityModel, relations, _ns, _enumNamespaces, _isSnakeCase)
                    .WriteToFile(Path.Combine(folderPath, $"{entityModel.Name}.g.cs"));
                // todo зачем это делать в цикле?
                files = files.Where(f => f.Name != entityModel.Name).ToArray();
            }

            foreach (var file in files) File.Delete(file.FilePath);
        }
    }
}