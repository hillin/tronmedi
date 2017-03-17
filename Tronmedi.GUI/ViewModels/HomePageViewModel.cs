using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tronmedi.GUI.ViewModels.BrowserPage;
using Tronmedi.GUI.ViewModels.ScanPage;

namespace Tronmedi.GUI.ViewModels
{
    internal class HomePageViewModel : PageViewModel
    {
        public override string PageTitle => "Tronmedi";
        public RootActionViewModel[] RootActions => MainViewModel.Current.RootActions;
    }
}
