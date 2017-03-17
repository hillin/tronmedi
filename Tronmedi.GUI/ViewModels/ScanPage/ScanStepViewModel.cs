using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
    class ScanStepViewModel : PropertyChangedBase
    {
        public string Title { get; set; }
        public ImageSource Icon { get; set; }
        public string IconKey { get; set; }
        public ScanSubpageViewModel ViewModel { get; set; }


        private bool _isDone;
        public bool IsDone
        {
            get { return _isDone; }
            set
            {
                _isDone = value;
                this.NotifyOfPropertyChange(nameof(this.IsDone));
            }
        }

    }
}
