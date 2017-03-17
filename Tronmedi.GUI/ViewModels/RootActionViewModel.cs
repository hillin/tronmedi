using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Tronmedi.GUI.ViewModels
{
    internal class RootActionViewModel : PropertyChangedBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public PageViewModel ViewModel { get; set; }

        public void Start()
        {
            if (this.ViewModel != null)
            {
                MainViewModel.Current.Content = this.ViewModel;
            }
        }
    }
}
