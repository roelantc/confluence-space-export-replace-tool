using System.IO.Compression;

namespace ConfluenceSpaceExportReplaceTool.Logic;

internal static class ConfluenceSpaceImportGenerator
{
    /// <summary>
    /// Generates a zip file based on an export of a confluence space zip where the space keys are replaced
    /// </summary>
    /// <param name="zipFile"></param>
    /// <param name="extractionPath"></param>
    /// <param name="oldSpaceKey"></param>
    /// <param name="newSpaceKey"></param>
    /// <returns></returns>
    internal static string GenerateConfluenceSpaceImportZip(string zipFile, string extractionPath, string oldSpaceKey, string newSpaceKey)
    {
        // Generate new zip archive name
        var zipFileName = GenerateZipFileName(newSpaceKey);

        // Extract the export zip so we can work with the contents
        ExtractZipFile(extractionPath, zipFile);

        // Replace 'entities.xml' and 'exportDescriptor.properties' values
        ReplaceFiles(extractionPath, oldSpaceKey, newSpaceKey);

        // Zip the directory to a new confluence space import zip file
        ArchiveZipFile(extractionPath, zipFileName);

        // Return full path of newly zipped file
        return Path.GetFullPath(zipFileName);
    }

    private static string GenerateZipFileName(string spaceKey)
    {
        return $"Confluence-export-space-{spaceKey.ToLowerInvariant()}.zip";
    }

    private static void ExtractZipFile(string extractionPath, string zipPath)
    {
        using var zipArchive = ZipFile.OpenRead(zipPath);
        zipArchive.ExtractToDirectory(extractionPath);
    }

    private static void ReplaceFiles(string path, string oldSpaceKey, string newSpaceKey)
    {
        if (!FileExists(path, Configuration.Constants.EntitiesXmlFile) || !FileExists(path, Configuration.Constants.ExportDescriptorPropertiesFile))
        {
            Console.WriteLine($"The '{Configuration.Constants.EntitiesXmlFile}' or '{Configuration.Constants.ExportDescriptorPropertiesFile}' file does not exist.");
            return;
        }

        // Replace space keys and write to file 'entities.xml' 
        var entitiesXml = EntitiesXmlReplacer.ReplaceEntitiesXmlSpaceKey(GetFileContents(path, Configuration.Constants.EntitiesXmlFile), oldSpaceKey, newSpaceKey);
        ReplaceFile(path, Configuration.Constants.EntitiesXmlFile, entitiesXml);

        // Replace space keys and write to file 'exportDescriptor.properties'
        var exportDescriptorProperties =
            ExportDescriptorPropertiesReplacer.ReplaceExportDescriptorPropertiesSpaceKey(GetFileContents(path, Configuration.Constants.ExportDescriptorPropertiesFile), oldSpaceKey, newSpaceKey);
        ReplaceFile(path, Configuration.Constants.ExportDescriptorPropertiesFile, exportDescriptorProperties);
    }

    private static string GetFileContents(string path, string fileName)
    {
        return File.ReadAllText($@"{path}\{fileName}");
    }

    private static bool FileExists(string path, string fileName)
    {
        return File.Exists($@"{path}\{fileName}");
    }

    private static void ReplaceFile(string path, string fileName, string contents)
    {
        File.Delete($@"{path}\{fileName}");
        File.WriteAllText($@"{path}\{fileName}", contents);
    }

    private static void ArchiveZipFile(string extractionPath, string zipFileName)
    {
        ZipFile.CreateFromDirectory(extractionPath, zipFileName);
        Directory.Delete(extractionPath, true);
    }
}