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
                }

                var assembly = Assembly.LoadFrom(Path.Combine(setting.Root, setting.Dll));
                Gen(assembly, setting);
            }
        }
        
        private static void Gen(Assembly assembly, GeneratorSettings setting)
        {
            Log("Started");
            Log($"Solution - {Path.Combine(setting.Root, setting.CsProject)}.");

            Log("Create db domain");
            var dbEntityDomain = new DefaultDbEntityDomain(assembly);

            Log("Generate db entities");
            var entityModels = dbEntityDomain.EntityModels;
            var relations = entityModels.CollectRelations();

            var modelNamespace = setting.DbModels.Namespace;
            var enumNamespace = setting.DbModels.EnumNamespace;
            var dbEntitiesGenerator = new DbEntitiesGenerator(entityModels, relations, modelNamespace, enumNamespace);

            var folderPath = Path.Combine(setting.Root, setting.DbModels.Path);
            Directory.CreateDirectory(folderPath);
            dbEntitiesGenerator.Generate(folderPath);

            if (setting.DbFluentFk != null)
            {
                Log("Generate foreign keys by fluent syntax");
                var ns = setting.DbFluentFk.Namespace;
                var dbForeignKeyFluentGenerator = new DbForeignKeyFluentGenerator(relations, ns, modelNamespace);
                dbForeignKeyFluentGenerator.WriteToFile(Path.Combine(setting.Root, setting.DbFluentFk.Path));
            }

            if (setting.DbFluentEnum != null)
            {
                Log("Generate enum by fluent syntax");
                var ns = setting.DbFluentEnum.Namespace;
                var enumFluentGenerator = new EnumFluentGenerator(entityModels, ns, enumNamespace);
                enumFluentGenerator.WriteToFile(Path.Combine(setting.Root, setting.DbFluentEnum.Path));
            }

            if (setting.Dto != null)
            {
                Log("Create dto domain");
                var dtoEntityDomain = new DefaultDtoEntityDomain(assembly);
                var models = dtoEntityDomain.EntityModels
                    .Where(dto => dto.Attributes.Any(a => a.GetType().Name == nameof(CreateDtoAttribute)))
                    .Select(dto => (IEntityModel) new EntityModelDtoIgnore(dto))
                    .ToArray();

                Log("Generate dtos");
                var ns = setting.Dto.Namespace;
                var dtoEntitiesGenerator = new DtoEntitiesGenerator(models, ns, enumNamespace);
                folderPath = Path.Combine(setting.Root, setting.Dto.Path);
                Directory.CreateDirectory(folderPath);
                dtoEntitiesGenerator.Generate(folderPath);
            }
        }

        private static void Log(string str) => Console.WriteLine(str);
    }
}