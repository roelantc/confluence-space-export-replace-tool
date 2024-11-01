using System.Text.RegularExpressions;
using ConfluenceSpaceExportReplaceTool.Configuration;
using ConfluenceSpaceExportReplaceTool.Logic;

namespace ConfluenceSpaceExportReplaceTool;

internal static partial class Program
{
    internal static void Main()
    {
        Console.WriteLine(@"Example input: '.\Confluence-export-space-[OldSpaceKey].zip'");
        Console.WriteLine("Provide the path where to find the confluence space export:");
        var zipFilePath = Console.ReadLine();
        
        if (string.IsNullOrEmpty(zipFilePath))
        {
            Console.WriteLine("Invalid path to the .zip file provided");
            return;
        }
        
        if (IsNotAZipFile(zipFilePath))
        {
            Console.WriteLine("Invalid zip file provided");
            return;
        }

        if (DoesFileNotExist(zipFilePath))
        {
            Console.WriteLine("Zip file provided does not exist");
            return;
        }

        Console.WriteLine("The space key should be composed of only alphanumeric characters (a-z, A-Z, 0-9) and should be no more than 255 characters in length");
        Console.WriteLine("Provide old space key:");
        var oldSpaceKey = Console.ReadLine()?.ToUpperInvariant();

        if (string.IsNullOrEmpty(oldSpaceKey))
        {
            Console.WriteLine("Old space key cannot be null or empty");
            return;
        }

        if (IsInvalidSpaceKey(oldSpaceKey))
        {
            Console.WriteLine("Old space key is invalid and should be composed of only alphanumeric characters (a-z, A-Z, 0-9) and should be no more than 255 characters in length");
            return;
        }

        Console.WriteLine("The space key should be composed of only alphanumeric characters (a-z, A-Z, 0-9) and should be no more than 255 characters in length");
        Console.WriteLine("Provide new space key:");
        var newSpaceKey = Console.ReadLine()?.ToUpperInvariant();

        if (string.IsNullOrEmpty(newSpaceKey))
        {
            Console.WriteLine("New space key cannot be null or empty");
            return;
        }

        if (IsInvalidSpaceKey(newSpaceKey))
        {
            Console.WriteLine("New space key is invalid and should be composed of only alphanumeric characters (a-z, A-Z, 0-9) and should be no more than 255 characters in length");
            return;
        }
        
        var zipExtractionDirectory = Path.GetFullPath(Constants.ZipExtractionDirectory);
        var confluenceSpaceExportZip = Path.GetFullPath(zipFilePath);
        var confluenceSpaceImportZip = ConfluenceSpaceImportGenerator.GenerateConfluenceSpaceImportZip(confluenceSpaceExportZip, zipExtractionDirectory, oldSpaceKey, newSpaceKey);

        Console.WriteLine($"Your Confluence space import zip file can be found here: {Path.GetFullPath(confluenceSpaceImportZip)}");
        Console.ReadLine();
    }

    private static bool IsInvalidSpaceKey(string spaceKey)
    {
        return !SpaceKeyRegex().IsMatch(spaceKey);
    }

    private static bool DoesFileNotExist(string filePath)
    {
        return !File.Exists(filePath);
    }

    private static bool IsNotAZipFile(string filePath)
    {
        return !filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex(RegexPatterns.SpaceKeyRegexPattern)]
    private static partial Regex SpaceKeyRegex();
}