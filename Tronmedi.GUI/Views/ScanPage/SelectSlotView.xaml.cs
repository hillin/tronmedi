using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Tronmedi.GUI.ViewModels.ScanPage;

namespace Tronmedi.GUI.Views.ScanPage
{
    public partial class SelectSlotView : UserControl
    {
        private SelectSlotViewModel ViewModel => this.DataContext as SelectSlotViewModel;

        private DialogSession _loadingDialogSession;

        public SelectSlotView()
        {
            this.InitializeComponent();
            this.DataContextChanged += this.SelectSlotView_DataContextChanged;
        }

        private void SelectSlotView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ViewModel == null)
                return;

            this.ViewModel.PropertyChanged += this.ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.ViewModel.IsPrescanning))
            {
                if (this.ViewModel.IsPrescanning)
                {
#pragma warning disable 4014
                    DialogHost.Show(this.FindResource("LoadingDialogContent"), delegate (object _, DialogOpenedEventArgs args)
                    {
                        _loadingDialogSession = args.Session;
                    });
#pragma warning restore 4014
                }
                else
                {
                    _loadingDialogSession?.Close();
                    _loadingDialogSession = null;
                }
            }
        }

        private void StartPrescan_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel?.SelectedSlotCount == 0)
            {
                this.NoSlotSelectedMessageSnackbar.IsActive = true;
            }
        }

        private void NoSlotSelectedMessageSnackbar_ActionClick(object sender, RoutedEventArgs e)
        {
            this.NoSlotSelectedMessageSnackbar.IsActive = false;
        }
    }
}
