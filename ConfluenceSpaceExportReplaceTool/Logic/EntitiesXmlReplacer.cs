namespace ConfluenceSpaceExportReplaceTool.Logic;

internal class EntitiesXmlReplacer
{
    /// <summary>
    ///     Replaces the space keys in 'entities.xml' for the space so there can be a unique new space in Confluence
    ///     Based on solution 3:
    ///     https://confluence.atlassian.com/confkb/how-to-copy-or-rename-a-space-in-confluence-169578.html
    /// </summary>
    /// <param name="entitiesXml"></param>
    /// <param name="oldSpaceKey"></param>
    /// <param name="newSpaceKey"></param>
    /// <returns></returns>
    internal static string ReplaceEntitiesXmlSpaceKey(string entitiesXml, string oldSpaceKey, string newSpaceKey)
    {
        // Make sure the space keys are uppercase because they should be that way in order for the configuration to work
        oldSpaceKey = oldSpaceKey.ToUpperInvariant();
        newSpaceKey = newSpaceKey.ToUpperInvariant();

        entitiesXml = entitiesXml.Replace(oldSpaceKey, newSpaceKey);
        entitiesXml = entitiesXml.Replace($"spaceKey={oldSpaceKey}", $"spaceKey={newSpaceKey}");
        entitiesXml = entitiesXml.Replace($"[{oldSpaceKey}:", $"[{newSpaceKey}:");
        entitiesXml = entitiesXml.Replace($"key={oldSpaceKey}]", $"key={newSpaceKey}]");
        entitiesXml = entitiesXml.Replace($"<spaceKey>{oldSpaceKey}</spaceKey>", $"<spaceKey>{newSpaceKey}</spaceKey>");

        // TODO: Make this optional via an option
        // Perform this step if links within the renamed space should point to themselves and not the original space key, otherwise, skip
        entitiesXml = entitiesXml.Replace($"ri:space-key=\"{oldSpaceKey}\"", $"ri:space-key=\"{newSpaceKey}\"");
        entitiesXml = entitiesXml.Replace($"ri:space-key={oldSpaceKey}", $"ri:space-key={newSpaceKey}");

        entitiesXml = entitiesXml.Replace($"<ac:parameter ac:name=\"spaces\">{oldSpaceKey}</ac:parameter>", $"<ac:parameter ac:name=\"spaces\">{newSpaceKey}</ac:parameter>");
        entitiesXml = entitiesXml.Replace($"<ac:parameter ac:name=\"spaceKey\">{oldSpaceKey}</ac:parameter>", $"<ac:parameter ac:name=\"spaceKey\">{newSpaceKey}</ac:parameter>");
        entitiesXml = entitiesXml.Replace($"<ac:parameter ac:name=\"spaceKey\">{oldSpaceKey}</ac:parameter>", $"<ac:parameter ac:name=\"spaceKey\">{newSpaceKey}</ac:parameter>");

        // Beware new key is used on both sides, that is how it should be
        entitiesXml = entitiesXml.Replace($"<property name=\"lowerDestinationSpaceKey\"><![CDATA[{newSpaceKey}]]></property>",
            $"<property name=\"lowerDestinationSpaceKey\"><![CDATA[{newSpaceKey.ToLowerInvariant()}]]></property>");
        entitiesXml = entitiesXml.Replace($"<property name=\"lowerKey\"><![CDATA[{newSpaceKey}]]></property>", $"<property name=\"lowerKey\"><![CDATA[{newSpaceKey.ToLowerInvariant()}]]></property>");

        entitiesXml = entitiesXml.Replace($"<property name=\"lowerKey\"><![CDATA[{oldSpaceKey.ToLowerInvariant()}]]></property>",
            $"<property name=\"lowerKey\"><![CDATA[{newSpaceKey.ToLowerInvariant()}]]></property>");
        entitiesXml = entitiesXml.Replace($"spaceKey={oldSpaceKey}", $"spaceKey={newSpaceKey}");
        entitiesXml = entitiesXml.Replace($"spacekey={oldSpaceKey.ToLowerInvariant()}", $"spacekey={newSpaceKey.ToLowerInvariant()}");

        return entitiesXml;
    }
}