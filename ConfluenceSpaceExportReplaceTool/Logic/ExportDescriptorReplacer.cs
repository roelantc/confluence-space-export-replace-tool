namespace ConfluenceSpaceExportReplaceTool.Logic;

internal static class ExportDescriptorPropertiesReplacer
{
    /// <summary>
    ///     Replaces the space keys in 'exportDescriptor.properties' for the space so there can be a unique new space in Confluence
    ///     Based on solution 3:
    ///     https://confluence.atlassian.com/confkb/how-to-copy-or-rename-a-space-in-confluence-169578.html
    /// </summary>
    /// <param name="exportDescriptorProperties"></param>
    /// <param name="oldSpaceKey"></param>
    /// <param name="newSpaceKey"></param>
    /// <returns></returns>
    internal static string ReplaceExportDescriptorPropertiesSpaceKey(string exportDescriptorProperties, string oldSpaceKey, string newSpaceKey)
    {
        // Make sure the space keys are uppercase because they should be that way in order for the configuration to work
        oldSpaceKey = oldSpaceKey.ToUpperInvariant();
        newSpaceKey = newSpaceKey.ToUpperInvariant();

        return exportDescriptorProperties.Replace(oldSpaceKey, newSpaceKey);
    }
}