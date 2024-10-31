using System.IO.Compression;
using System.Text.RegularExpressions;
using ConfluenceSpaceExportReplaceTool.Replacers;

namespace ConfluenceSpaceExportReplaceTool;

internal static partial class Program
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

        if (!zipPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Invalid zip file provided");
            return;
        }

        if (!File.Exists(zipPath))
        {
            Console.WriteLine("Zip file provided does not exist");
            return;
        }
        
        using var zipArchive = ZipFile.OpenRead(zipPath);
        
        // Validation so that the file contains 
        if (zipArchive.DoesNotContainRequiredFiles())
        {
            Console.WriteLine("Zip archive does not contain required 'entities.xml' or 'exportDescriptor.properties' file");
            return;
        }

        // Extract zip in order to work easily with the content
        zipArchive.ExtractToDirectory(Constants.Configuration.ZipExtractionDirectory);

        var destinationArchiveFileName = DestinationArchiveFileName(oldSpaceKey, newSpacekey);

        Console.WriteLine($"Your Confluence space import zip file can be found here: {Path.GetFullPath(destinationArchiveFileName)}");
    }

    private static bool DoesNotContainRequiredFiles(this ZipArchive zipArchive)
    {
        return zipArchive.Entries.All(x => x.FullName != Constants.Configuration.EntitiesXmlFile) || 
               zipArchive.Entries.All(x => x.FullName != Constants.Configuration.ExportDescriptorPropertiesFile);
    }

    private static string DestinationArchiveFileName(string oldSpaceKey, string newSpacekey)
    {
        // Replace space keys and write to file 'entities.xml' 
        var entitiesXml = File.ReadAllText($@"{Constants.Configuration.ZipExtractionDirectory}\{Constants.Configuration.EntitiesXmlFile}");
        var replacedEntitiesXml = EntitiesXmlReplacer.ReplaceEntitiesXmlSpaceKey(entitiesXml, oldSpaceKey, newSpacekey);

        File.Delete($@"{Constants.Configuration.ZipExtractionDirectory}\{Constants.Configuration.EntitiesXmlFile}");
        File.WriteAllText($@"{Constants.Configuration.ZipExtractionDirectory}\{Constants.Configuration.EntitiesXmlFile}", replacedEntitiesXml);

        // Replace space keys and write to file 'exportDescriptor.properties'
        var exportDescriptorProperties = File.ReadAllText($@"{Constants.Configuration.ZipExtractionDirectory}\{Constants.Configuration.ExportDescriptorPropertiesFile}");
        var replacedExportDescriptorProperties = ExportDescriptorPropertiesReplacer.ReplaceExportDescriptorPropertiesSpaceKey(exportDescriptorProperties, oldSpaceKey, newSpacekey);

        File.Delete($@"{Constants.Configuration.ZipExtractionDirectory}\{Constants.Configuration.ExportDescriptorPropertiesFile}");
        File.WriteAllText($@"{Constants.Configuration.ZipExtractionDirectory}\{Constants.Configuration.ExportDescriptorPropertiesFile}", replacedExportDescriptorProperties);

        // Zip the directory to a new confluence space import zip
        var destinationArchiveFileName = $"Confluence-export-space-{newSpacekey.ToLowerInvariant()}.zip";
        ZipFile.CreateFromDirectory(Constants.Configuration.ZipExtractionDirectory, destinationArchiveFileName);
        
        // Delete the directory recursively
        Directory.Delete(Constants.Configuration.ZipExtractionDirectory, true);
        return destinationArchiveFileName;
    }

    [GeneratedRegex(Constants.RegexPatterns.SpaceKeyRegexPattern)]
    private static partial Regex SpaceKeyRegex();
}