using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;

namespace Tronmedi.Camera
{
	using Size = System.Drawing.Size;

    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl
    {
        public CameraControl()
        {
            InitializeComponent();

            Dispatcher.ShutdownStarted += OnShutdownStarted; 
        }

        private DirectShowProxy _proxy;

        private DirectShowProxy Proxy
        {
            get { return _proxy ?? (_proxy = new DirectShowProxy()); }
        }

        private readonly List<CameraId> _captureDevices = new List<CameraId>();
        private void SaveVideoDevice(ref DirectShowProxy.VideoInputDeviceInfo info)
        {
            if (!string.IsNullOrEmpty(info.DevicePath))
            {
                _captureDevices.Add(new CameraId(info));
            }
        }

        /// <summary>
        /// Gets a list of available video capture devices.
        /// </summary>
        /// <exception cref="Win32Exception">Failed to load the DirectShow utilities dll.</exception>
        public IEnumerable<CameraId> GetVideoCaptureDevices()
        {
            _captureDevices.Clear();
            Proxy.EnumVideoInputDevices(SaveVideoDevice);

            return new List<CameraId>(_captureDevices);
        }

        /// <summary>
        /// Initializes a capture graph.
        /// </summary>
        /// <exception cref="Win32Exception">Failed to load the DirectShow utilities dll.</exception>
        /// <exception cref="DirectShowException">Failed to setup a capture graph.</exception>
        private void InitializeCaptureGraph()
        {
            Proxy.BuildCaptureGraph();
            Proxy.AddRenderFilter(VideoWindow.Handle);
        }

        private static readonly DependencyPropertyKey IsCapturingPropertyKey
            = DependencyProperty.RegisterReadOnly("IsCapturing", typeof(bool), typeof(CameraControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsCapturingProperty
            = IsCapturingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating whether the control is capturing a video stream.
        /// </summary>
        [Browsable(false)]
        public bool IsCapturing
        {
            get { return (bool)GetValue(IsCapturingProperty); }
            private set { SetValue(IsCapturingPropertyKey, value); }
        }

        private bool _captureGraphInitialized;
        private CameraId _currentCamera;

        /// <summary>
        /// Starts a capture.
        /// </summary>
        /// <param name="camera">The camera to capture from.</param>
        /// <exception cref="ArgumentNullException">A null reference is passed as an argument.</exception>
        /// <exception cref="Win32Exception">Failed to load the DirectShow utilities dll.</exception>
        /// <exception cref="DirectShowException">Failed to run a video capture graph.</exception>
        public void StartCapture(CameraId camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException();
            }

            if (!_captureGraphInitialized)
            {
                InitializeCaptureGraph();

                _captureGraphInitialized = true;
            }

            if (IsCapturing)
            {
                if (_currentCamera == camera)
                {
                    return;
                }

                StopCapture();
            }

            if (_currentCamera != null)
            {
                Proxy.ResetCaptureGraph();
                _currentCamera = null;
            }

            Proxy.AddCaptureFilter(camera.DevicePath);
            _currentCamera = camera;

            try
            {
                Proxy.Start();
                IsCapturing = true;
            }
            catch (DirectShowException)
            {
                Proxy.ResetCaptureGraph();
                _currentCamera = null;
                throw;
            }
        }

        /// <summary>
        /// Retrieves the unstretched image being captured.
        /// </summary>
        /// <returns>The current image.</returns>
        /// <exception cref="InvalidOperationException">The control is not capturing a video stream.</exception>
        /// <exception cref="DirectShowException">Failed to get the current image.</exception>
        public Bitmap GetCurrentImage()
        {
            if (!IsCapturing)
            {
                throw new InvalidOperationException();
            }

            return Proxy.GetCurrentImage();
        }

        /// <summary>
        /// Gets the unstretched video size.
        /// </summary>
        [Browsable(false)]
        public Size VideoSize
        {
            get { return IsCapturing ? Proxy.GetVideoSize() : new Size(0, 0); }
        }

        /// <summary>
        /// Stops a capture.
        /// </summary>
        /// <exception cref="InvalidOperationException">The control is not capturing a video stream.</exception>
        /// <exception cref="DirectShowException">Failed to stop a video capture graph.</exception>
        public void StopCapture()
        {
            if (!IsCapturing)
            {
                throw new InvalidOperationException();
            }

            Proxy.Stop();
            IsCapturing = false;

            Proxy.ResetCaptureGraph();
            _currentCamera = null;
        }

        private void OnShutdownStarted(object sender, EventArgs eventArgs)
        {
            if (_proxy != null)
            {
                if (IsCapturing)
                {
                    StopCapture();
                }

                _proxy.DestroyCaptureGraph();
                _proxy.Dispose();
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (oldContent != null)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
