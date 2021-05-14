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
        static void Main(string[] args)
        {
            var configFileName = "settings.json";
            if (args.Length > 0)
            {
                if (File.Exists(args[0]))
                {
                    configFileName = args[0];
                }
                else
                {
                    throw new InvalidProgramException("Configuration not found: " + args[0]);
                }
            }

            var projectSettingsText = File.ReadAllText(configFileName);
            var projectSettings = JsonSerializer.Deserialize<GenProjectSettings[]>(projectSettingsText);
            var currentDirectory = Environment.CurrentDirectory;

            foreach (var projectSetting in projectSettings)
            {
                Environment.CurrentDirectory = projectSetting.Root;
                var settingsText = File.ReadAllText(projectSetting.ConfigName);
                var settings = JsonSerializer.Deserialize<GeneratorSettings>(settingsText);
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/C dotnet publish {settings.CsProject}"
                };
                using (var process = new Process {StartInfo = processStartInfo})
                {
                    process.Start();
                    process.WaitForExit();
                }

                var assembly = Assembly.LoadFrom(settings.Dll);
                Gen(assembly, settings);
                Environment.CurrentDirectory = currentDirectory;
            }
        }

        private static void Gen(Assembly assembly, GeneratorSettings setting)
        {
            Log("Started");
            Log($"Solution - {setting.CsProject}.");

            Log("Create db domain");
            var dbEntityDomain = new DefaultDbEntityDomain(assembly);

            Log("Generate db entities");
            var entityModels = dbEntityDomain.EntityModels;
            var relations = entityModels.CollectRelations();

            var modelNamespace = setting.DbModels.Namespace;
            var enumNamespaces = setting.DbModels.EnumNamespaces;
            var isSnakeCase = setting.DbModels.IsSnakeCase;
            var dbEntitiesGenerator =
                new DbEntitiesGenerator(entityModels, relations, modelNamespace, enumNamespaces, isSnakeCase);

            var folderPath = setting.DbModels.Path;
            Directory.CreateDirectory(folderPath);
            dbEntitiesGenerator.Generate(folderPath);

            if (setting.DbFluentFk != null)
            {
                Log("Generate foreign keys by fluent syntax");
                var ns = setting.DbFluentFk.Namespace;
                var dbForeignKeyFluentGenerator = new DbForeignKeyFluentGenerator(relations, ns, modelNamespace);
                dbForeignKeyFluentGenerator.WriteToFile(setting.DbFluentFk.Path);
            }

            if (setting.DbFluentIndex != null)
            {
                Log("Generate index by fluent syntax");
                var ns = setting.DbFluentIndex.Namespace;
                var dbForeignKeyFluentGenerator = new DbIndexFluentGenerator(dbEntityDomain, ns, modelNamespace);
                dbForeignKeyFluentGenerator.WriteToFile(setting.DbFluentIndex.Path);
            }

            if (setting.DbFluentEnum != null)
            {
                Log("Generate enum by fluent syntax");
                var ns = setting.DbFluentEnum.Namespace;
                var enumFluentGenerator = new EnumFluentGenerator(entityModels, ns, enumNamespaces);
                enumFluentGenerator.WriteToFile(setting.DbFluentEnum.Path);
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
                var dtoEntitiesGenerator = new DtoEntitiesGenerator(models, ns, enumNamespaces);
                folderPath = setting.Dto.Path;
                Directory.CreateDirectory(folderPath);
                dtoEntitiesGenerator.Generate(folderPath);
            }
        }

        private static void Log(string str) => Console.WriteLine(str);
    }
}