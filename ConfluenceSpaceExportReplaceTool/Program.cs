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
        
        if (!zipFilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Invalid zip file provided");
            return;
        }

        if (!File.Exists(zipFilePath))
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

        if (!SpaceKeyRegex().IsMatch(oldSpaceKey))
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

        if (!SpaceKeyRegex().IsMatch(newSpaceKey))
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

    [GeneratedRegex(RegexPatterns.SpaceKeyRegexPattern)]
    private static partial Regex SpaceKeyRegex();
}