using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class ScanSubpageViewModel : PropertyChangedBase
    {
        public ScanContext ScanContext { get; private set; }
        public ScanPageViewModel Owner { get; set; }

        public virtual void GoBack()
        {
            this.Owner.GoBack();
        }

        public virtual void NextStep()
        {
            this.Owner.NextStep();
        }

        public virtual void Initialize(ScanContext scanContext)
        {
            this.ScanContext = scanContext;
        }

        public void Browse(string fileName)
        {
            this.Owner.Browse(fileName);
        }
    }
}
