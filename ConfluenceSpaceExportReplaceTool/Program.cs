using System.IO.Compression;
using System.Text.RegularExpressions;
using ConfluenceSpaceExportReplaceTool.Replacers;

namespace ConfluenceSpaceExportReplaceTool;

internal abstract partial class Program
{
    internal static void Main()
    {
        Console.WriteLine(@"Example input: '.\Confluence-export-space-[OldSpaceKey].zip'");
        Console.WriteLine("Provide path where to find the confluence space export:");
        var zipPath = Console.ReadLine();

        if (string.IsNullOrEmpty(zipPath))
        {
            Console.WriteLine("Invalid path to the .zip file provided");
            return;
        }
        
        Console.WriteLine("Provide old space key:");
        var oldSpaceKey = Console.ReadLine()?.ToUpperInvariant();
        
        if (string.IsNullOrEmpty(oldSpaceKey))
        {
            Console.WriteLine("Old space key cannot be null or empty");
            return;
        }

        if (!SpaceKeyRegex().IsMatch(oldSpaceKey))
        {
            Console.WriteLine("Old space key is invalid \n\n Composed of only alphanumeric characters (a-z, A-Z, 0-9) \n\n No more than 255 characters in length");
            return;
        }

        Console.WriteLine("Provide new space key:");
        var newSpacekey = Console.ReadLine()?.ToUpperInvariant();

        if (string.IsNullOrEmpty(newSpacekey))
        {
            Console.WriteLine("New space key cannot be null or empty");
            return;
        }
        
        if (!SpaceKeyRegex().IsMatch(newSpacekey))
        {
            Console.WriteLine("New space key is invalid \n\n Composed of only alphanumeric characters (a-z, A-Z, 0-9) \n\n No more than 255 characters in length");
            return;
        }

        using (var zipArchive = ZipFile.OpenRead(zipPath))
        {
            // Validation so that the file contains 
            if (zipArchive.Entries.All(x => x.FullName != Constants.Configuration.EntitiesXmlFile) || zipArchive.Entries.All(x => x.FullName != Constants.Configuration.ExportDescriptorPropertiesFile))
            {
                Console.WriteLine("Zip archive does not contain required 'entities.xml' or 'exportDescriptor.properties' file");
                return;
            }

            // Extract zip in order to work easily with the content
            zipArchive.ExtractToDirectory(Constants.Configuration.ExtractionDirectory);
        }

        // Replace space keys and write to file 'entities.xml' 
        var entitiesXml = File.ReadAllText($@"{Constants.Configuration.ExtractionDirectory}\{Constants.Configuration.EntitiesXmlFile}");
        var replacedEntitiesXml = EntitiesXmlReplacer.ReplaceEntitiesXmlSpaceKey(entitiesXml, oldSpaceKey, newSpacekey);

        File.Delete($@"{Constants.Configuration.ExtractionDirectory}\{Constants.Configuration.EntitiesXmlFile}");
        File.WriteAllText($@"{Constants.Configuration.ExtractionDirectory}\{Constants.Configuration.EntitiesXmlFile}", replacedEntitiesXml);

        // Replace space keys and write to file 'exportDescriptor.properties'
        var exportDescriptorProperties = File.ReadAllText($@"{Constants.Configuration.ExtractionDirectory}\{Constants.Configuration.ExportDescriptorPropertiesFile}");
        var replacedExportDescriptorProperties = ExportDescriptorPropertiesReplacer.ReplaceExportDescriptorPropertiesSpaceKey(exportDescriptorProperties, oldSpaceKey, newSpacekey);

        File.Delete($@"{Constants.Configuration.ExtractionDirectory}\{Constants.Configuration.ExportDescriptorPropertiesFile}");
        File.WriteAllText($@"{Constants.Configuration.ExtractionDirectory}\{Constants.Configuration.ExportDescriptorPropertiesFile}", replacedExportDescriptorProperties);

        // Zip the directory to a new confluence space import zip
        var destinationArchiveFileName = $"Confluence-export-space-{newSpacekey.ToLowerInvariant()}.zip";
        ZipFile.CreateFromDirectory(Constants.Configuration.ExtractionDirectory, destinationArchiveFileName);

        Console.WriteLine($"Your Confluence space import zip file can be found here: {Path.GetFullPath(destinationArchiveFileName)}");

        // Delete the directory recursively
        Directory.Delete(Constants.Configuration.ExtractionDirectory, true);
    }

    [GeneratedRegex(Constants.RegexPatterns.SpaceKeyRegexPattern)]
    private static partial Regex SpaceKeyRegex();
}