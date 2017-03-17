using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class CompletedViewModel : SlotSubpageViewModelBase
    {
        public CompletedViewModel()
            : base(SlotDisplayMode.View)
        {

        }

        public void StartNewScan()
        {
            this.Owner.StartNewScan();
        }

        public void Close()
        {
            this.Owner.GoHome();
        }
    }
}
