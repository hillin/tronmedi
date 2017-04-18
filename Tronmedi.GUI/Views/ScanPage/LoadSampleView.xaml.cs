using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Tronmedi.Camera;

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
			var identifier = $"@device:pnp:\\\\?\\usb#vid_{Config.Instance.CameraVid}&pid_{Config.Instance.CameraPid}";
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
				var path = device.DevicePath;

				try
				{
					this.Dispatcher.BeginInvoke(new Action(() =>
					{
						device = this.CameraControl.GetVideoCaptureDevices().First(d => d.DevicePath == path);
						this.CameraControl.StartCapture(device, Config.Instance.StillImage.Width, Config.Instance.StillImage.Height,
							Config.Instance.StillImage.ColorDepth);
						this.CameraErrorText.Text = string.Empty;
						this.CameraControl.Visibility = Visibility.Visible;

						this.Capture();
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

		private async void Capture()
		{
			using (var fileStream = File.Create("d:\\1.png"))
			{
				BitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(await this.CameraControl.TakeStillPicture()));
				encoder.Save(fileStream);
			}
		}
	}
}
