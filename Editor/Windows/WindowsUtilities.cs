namespace SZUtilities._Editor.Windows
{
    public static class WindowsUtilities
    {
        public static void ShowDirectoryInExplorer(string directory)
        {
            directory = directory.Replace(@"/", @"\");   // explorer doesn't like front slashes
            System.Diagnostics.Process.Start("explorer.exe", $"/select,{directory}");
        }
    }
}