using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    internal class LoadSampleViewModel : ScanSubpageViewModel
    {
        public void GoHome()
        {
            this.Owner.GoHome();
        }
    }
}
