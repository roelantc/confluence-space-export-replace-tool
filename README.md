# confluence-space-export-replace-tool
Confluence space export replacement tool in order to copy spaces easily

## How to use
- Export the desired space you want to copy. Instructions can be found here:
  - https://support.atlassian.com/confluence-cloud/docs/export-content-to-word-pdf-html-and-xml/
- Open the 'ConfluenceSpaceExportReplaceTool.sln' file and start the console application
- Follow the steps on the command line:
  - Provide the path of the exported space zip
  - Provide the space key that corresponds with the exported space zip
  - Provide a new unique space key ([A-Z],[0-9] and a maximum of 255 characters)
  - A new zip file will be generated
- Import your copy of your Confluence space. Instructions can be founde here:
  - https://support.atlassian.com/confluence-cloud/docs/import-a-confluence-cloud-space/
