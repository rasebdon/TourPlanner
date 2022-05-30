using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Client.UI.Services
{
    public interface ISaveFileDialogProvider
    {
        public string DefaultExt { get; set; }
        public string Filter { get; set; }
        public string? GetFileName();
    }
}
