using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Tronmedi.GUI.ViewModels.BrowserPage;
using Tronmedi.GUI.ViewModels.ScanPage;

namespace Tronmedi.GUI.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {


        public static MainViewModel Current { get; private set; }
        public static void Browse(string fileName)
        {
            MainViewModel.Current.Content = new BrowserPageViewModel(fileName);
        }

        public RootActionViewModel[] RootActions { get; } =
           {
            new RootActionViewModel
            {
                Title = "New Scan",
                Description = "Scan new image",
                IsEnabled = true,
                ViewModel = new ScanPageViewModel()
            },
            new RootActionViewModel
            {
                Title = "Browse",
                Description = "Browse existing images",
                IsEnabled = true,
                ViewModel = new BrowserPageViewModel(null)
            },
            new RootActionViewModel
            {
                Title = "Login",
                IsEnabled = false
            },
            new RootActionViewModel
            {
                Title = "Calibrate",
                IsEnabled = false
            }
        };

        private PageViewModel _content;
        public PageViewModel Content
        {
            get { return _content; }
            set
            {
                _content = value;
                this.NotifyOfPropertyChange(nameof(this.Content));
            }
        }

        private bool _isDrawerOpen;
        public bool IsDrawerOpen
        {
            get { return _isDrawerOpen; }
            set
            {
                _isDrawerOpen = value;
                this.NotifyOfPropertyChange(() => this.IsDrawerOpen);
                this.Content.NotifyOfPropertyChange(() => this.Content.IsDrawerOpen);
            }
        }

        public MainViewModel()
        {
            MainViewModel.Current = this;
            this.Content = new HomePageViewModel();
        }

        public void GoHome()
        {
            MainViewModel.Current.Content = new HomePageViewModel();
        }
    }
}
