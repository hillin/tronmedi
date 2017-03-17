using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Tronmedi.GUI.ViewModels
{
    abstract class PageViewModel : PropertyChangedBase
    {
        public abstract string PageTitle { get; }
        public bool IsDrawerOpen
        {
            get { return MainViewModel.Current.IsDrawerOpen; }
            set
            {
                MainViewModel.Current.IsDrawerOpen = value;
                this.NotifyOfPropertyChange(() => this.IsDrawerOpen);
            }
        }

        public void GoHome()
        {
            MainViewModel.Current.GoHome();
        }
    }
}
