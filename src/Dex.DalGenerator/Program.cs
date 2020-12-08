using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Dex.DalGenerator.Common;
using Dex.DalGenerator.Core;
using Dex.DalGenerator.Core.Contracts.EntityModel;
using Dex.DalGenerator.Generator;
using Dex.DalGenerator.Settings;
using Dex.DalGenerator.Extensions;
using Dex.DalGenerator.Annotation.Attributes;
using Dex.DalGenerator.Templates;
using DbForeignKeyFluentGenerator = Dex.DalGenerator.Templates.DbForeignKeyFluentGenerator;
using EnumFluentGenerator = Dex.DalGenerator.Templates.EnumFluentGenerator;

namespace Dex.DalGenerator
{
    internal static class Program
    {
        static void Main()
        {
            var settings = JsonSerializer.Deserialize<GeneratorSettings[]>(File.ReadAllText("settings.json"));

            foreach (var setting in settings)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/C dotnet publish {Path.GetFullPath(setting.CsProject, setting.Root)}"
                };
                using (var process = new Process {StartInfo = processStartInfo})
                {
                    process.Start();
                    process.WaitForExit();
                    NewMethod(setting);
                }
            }
        }

        private static void NewMethod(GeneratorSettings settings)
        {
            var assembly = Assembly.LoadFrom(Path.Combine(settings.Root, settings.Dll));
            Log("Started");
            Log($"Solution - {Path.Combine(settings.Root, settings.CsProject)}.");

            Log("Create db domain");
            var dbEntityDomain = new DefaultDbEntityDomain(assembly);

            Log("Generate db entities");
            var entityModels = dbEntityDomain.EntityModels;
            var relations = entityModels.CollectRelations();

            var modelNamespace = settings.DbModels.Namespace;
            var enumNamespace = settings.DbModels.EnumNamespace;
            var dbEntitiesGenerator = new DbEntitiesGenerator(entityModels, relations, modelNamespace, enumNamespace);
            
            var folderPath = Path.Combine(settings.Root, settings.DbModels.Path);
            Directory.CreateDirectory(folderPath);
            dbEntitiesGenerator.Generate(folderPath);

            if (settings.DbFluentFk != null)
            {
                Log("Generate foreign keys by fluent syntax");
                var ns = settings.DbFluentFk.Namespace;
                var dbForeignKeyFluentGenerator = new DbForeignKeyFluentGenerator(relations, ns, modelNamespace);
                dbForeignKeyFluentGenerator.WriteToFile(Path.Combine(settings.Root, settings.DbFluentFk.Path));
            }
            
            if (settings.DbFluentEnum != null)
            {
                Log("Generate enum by fluent syntax");
                var ns = settings.DbFluentEnum.Namespace;
                var enumFluentGenerator = new EnumFluentGenerator(entityModels, ns, enumNamespace);
                enumFluentGenerator.WriteToFile(Path.Combine(settings.Root, settings.DbFluentEnum.Path));
            }
            
            if (settings.Dto != null)
            {
                Log("Create dto domain");
                var dtoEntityDomain = new DefaultDtoEntityDomain(assembly);
                var models = dtoEntityDomain.EntityModels
                    .Where(dto => dto.Attributes.Any(a => a.GetType().Name == nameof(CreateDtoAttribute)))
                    .Select(dto => (IEntityModel) new EntityModelDtoIgnore(dto))
                    .ToArray();
            
                Log("Generate dtos");
                var ns = settings.Dto.Namespace;
                var dtoEntitiesGenerator = new DtoEntitiesGenerator(models, ns, enumNamespace);
                folderPath = Path.Combine(settings.Root, settings.Dto.Path);
                Directory.CreateDirectory(folderPath);
                dtoEntitiesGenerator.Generate(folderPath);
            }
        }

        private static void Log(string str) => Console.WriteLine(str);
    }
}