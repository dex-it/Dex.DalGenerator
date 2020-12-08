using System.IO;
using Dex.DalGenerator.Core.Contracts;

namespace Dex.DalGenerator.Extensions
{
    internal static class GeneratorFileWriterExtension
    {
        public static void WriteToFile(this IEntityGenerator generator, string filePath)
        {
            var file = new FileInfo(filePath);
            file.Directory?.Create();
            File.WriteAllText(file.FullName, generator.Generate());
        }
    }
}