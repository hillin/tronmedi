using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using DirectShowLib;

namespace Tronmedi.Camera
{

	public partial class CameraControl
	{
		public CameraControl()
		{
			InitializeComponent();

			Dispatcher.ShutdownStarted += OnShutdownStarted;
			this.VideoWindow.SizeChanged += VideoWindow_SizeChanged;
		}

		private void VideoWindow_SizeChanged(object sender, Size e)
		{
			_currentCamera?.UpdateWindowSize((int) e.Width, (int) e.Height);
		}

		public IEnumerable<DsDevice> GetVideoCaptureDevices()
		{
			return DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
		}

		private static readonly DependencyPropertyKey IsCapturingPropertyKey
			= DependencyProperty.RegisterReadOnly("IsCapturing", typeof(bool), typeof(CameraControl),
				new FrameworkPropertyMetadata(false));

		public static readonly DependencyProperty IsCapturingProperty
			= IsCapturingPropertyKey.DependencyProperty;

		[Browsable(false)]
		public bool IsCapturing
		{
			get => (bool) GetValue(IsCapturingProperty);
			private set => SetValue(IsCapturingPropertyKey, value);
		}

		private Camera _currentCamera;

		public void StartCapture(DsDevice device, int width, int height, short colorDepth,
			CameraPerformanceMode mode = CameraPerformanceMode.Normal)
		{
			if (device == null)
			{
				throw new ArgumentNullException();
			}

			_currentCamera = new Camera(device, width, height, colorDepth, this.VideoWindow.Handle, mode);
			this.IsCapturing = true;
		}

		/// <summary>
		/// Stops a capture.
		/// </summary>
		/// <exception cref="InvalidOperationException">The control is not capturing a video stream.</exception>
		public void StopCapture()
		{
			if (!IsCapturing)
			{
				throw new InvalidOperationException();
			}

			_currentCamera.Dispose();
			_currentCamera = null;

			this.IsCapturing = false;
		}

		private void OnShutdownStarted(object sender, EventArgs eventArgs)
		{
			if (IsCapturing)
			{
				StopCapture();
			}
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			if (oldContent != null)
			{
				throw new InvalidOperationException();
			}
		}

		public Task<BitmapSource> TakeStillPicture()
		{
			if (!IsCapturing)
			{
				throw new InvalidOperationException();
			}

			return _currentCamera.Click();
		}

		public void SetProperty(CameraControlProperty property, CameraPropertyValue value)
		{
			_currentCamera.SetProperty(property, value);
		}

		public CameraPropertyRange GetPropertyRange(CameraControlProperty property)
		{
			return _currentCamera.GetPropertyRange(property);
		}

		public CameraPropertyValue GetProperty(CameraControlProperty property)
		{
			return _currentCamera.GetProperty(property);
		}
	}
}
