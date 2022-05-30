namespace TourPlanner.Client.UI.Services
{
    public class WindowsOpenFileDialogProvider : IOpenFileDialogProvider
    {
        public string DefaultExt { get; set; }
        public string Filter { get; set; }

        public string? GetFileName()
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDialog = new();

            openFileDialog.Filter = Filter;
            openFileDialog.DefaultExt = DefaultExt;

            if(openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}