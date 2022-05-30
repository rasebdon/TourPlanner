using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Client.UI.Services
{
    public class WindowsSaveFileDialogProvider : ISaveFileDialogProvider
    {
        public string DefaultExt { get; set; } = ".*";
        public string Filter { get; set; } = "";

        public string? GetFileName()
        {
            // Create SaveFileDialog
            Microsoft.Win32.SaveFileDialog saveFileDialog = new();

            saveFileDialog.Filter = Filter;
            saveFileDialog.DefaultExt = DefaultExt;

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }
    }
}
