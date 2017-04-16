using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Tronmedi.GUI.Views.ScanPage
{
	/// <summary>
	/// Interaction logic for LoadSampleView.xaml
	/// </summary>
	public partial class LoadSampleView : UserControl
	{

		public LoadSampleView()
		{
			InitializeComponent();
			this.Loaded += LoadSampleView_Loaded;
			this.Unloaded += LoadSampleView_Unloaded;
		}

		private void LoadSampleView_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.CameraControl.IsCapturing)
			{
				this.CameraControl.StopCapture();
			}
		}

		private void LoadSampleView_Loaded(object sender, RoutedEventArgs e)
		{
			this.CameraControl.Visibility = Visibility.Hidden;
			Task.Factory.StartNew(this.InitializeCamera);
		}

		private void InitializeCamera()
		{
			var devices = this.CameraControl.GetVideoCaptureDevices();
			var identifier = $"\\\\?\\usb#vid_{Config.Instance.CameraVid}&pid_{Config.Instance.CameraPid}";
			var device =
				devices.FirstOrDefault(d => d.DevicePath.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase));

			if (device == null)
			{
				this.Dispatcher.BeginInvoke(new Action(() =>
				{
					this.CameraErrorText.Text = "Cannot find Tronmedi camera device";
				}));
			}
			else
			{
				try
				{
					this.Dispatcher.BeginInvoke(new Action(() =>
					{
						this.CameraControl.StartCapture(device);
						this.CameraErrorText.Text = string.Empty;
						this.CameraControl.Visibility = Visibility.Visible;
					}));
				}
				catch (Exception e)
				{
					this.Dispatcher.BeginInvoke(new Action(() =>
					{
						this.CameraErrorText.Text = $"Error: failed to initialze Tronmedi camera device.\n\n{e.Message}";
					}));
				}
			}

			this.Dispatcher.BeginInvoke(new Action(() =>
			{
				this.CameraInitializationProgress.Visibility = Visibility.Collapsed;
			}));
		}
	}
}
