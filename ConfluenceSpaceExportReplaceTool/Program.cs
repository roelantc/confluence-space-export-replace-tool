using System.IO.Compression;
using System.Text.RegularExpressions;
using ConfluenceSpaceExportReplaceTool.Configuration;
using ConfluenceSpaceExportReplaceTool.Logic;

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
        
        var zipExtractionDirectory = Path.GetFullPath(Constants.ZipExtractionDirectory);
        var confluenceSpaceExportZip = Path.GetFullPath(zipPath);
        var confluenceSpaceImportZip = ConfluenceSpaceImportGenerator.GenerateConfluenceSpaceImportZip(confluenceSpaceExportZip, zipExtractionDirectory, oldSpaceKey, newSpacekey);

        Console.WriteLine($"Your Confluence space import zip file can be found here: {Path.GetFullPath(confluenceSpaceImportZip)}");
    }

    [GeneratedRegex(RegexPatterns.SpaceKeyRegexPattern)]
    private static partial Regex SpaceKeyRegex();
}