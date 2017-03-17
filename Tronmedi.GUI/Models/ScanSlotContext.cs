using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tronmedi.GUI.ViewModels.ScanPage;

namespace Tronmedi.GUI.Models
{
    class ScanSlotContext
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public LayerScanMode LayerScanMode { get; set; }
        public string FileName { get; set; }
        public bool ShouldUpload { get; set; }
        public ImageSource Image { get; set; }
    }
}
