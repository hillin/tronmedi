using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DirectShowLib;

namespace Tronmedi.Camera
{
	[SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
	public class Camera : ISampleGrabberCB, IDisposable
	{
		public CameraPerformanceMode Mode { get; }

		/// <summary> graph builder interface. </summary>
		private IFilterGraph2 _filterGraph;

		// Used to snap picture on Still pin
		private IAMVideoControl _vidControl;
		private IPin _pinStill;

		/// <summary> so we can wait for the async job to finish </summary>
		private readonly ManualResetEvent _pictureReady;

		private bool _wantOne;

		/// <summary> buffer for bitmap data.  Always release by caller</summary>
		private IntPtr _ipBuffer = IntPtr.Zero;


		public int Width { get; private set; }

		public int Height { get; private set; }

		public int Stride { get; private set; }

#if DEBUG
		// Allow you to "Connect to remote graph" from GraphEdit
		DsROTEntry _rot;
#endif

		// Zero based device index and device params and output window
		public Camera(DsDevice device, int width, int height, short depth, IntPtr handle, CameraPerformanceMode mode = CameraPerformanceMode.Normal)
		{
			this.Mode = mode;
			try
			{
				// Set up the capture graph
				this.SetupGraph(device, width, height, depth, handle);

				// tell the callback to ignore new images
				_pictureReady = new ManualResetEvent(false);
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		/// <summary> release everything. </summary>
		public void Dispose()
		{
#if DEBUG
			_rot?.Dispose();
#endif
			this.CloseInterfaces();
			_pictureReady?.Close();
		}

		~Camera()
		{
			this.Dispose();
		}

		public async Task<BitmapSource> Click()
		{
			// get ready to wait for new image
			_pictureReady.Reset();
			var size = Math.Abs(this.Stride) * this.Height;
			_ipBuffer = Marshal.AllocCoTaskMem(size);

			try
			{
				_wantOne = true;

				// If we are using a still pin, ask for a picture
				if (_vidControl != null)
				{
					// Tell the camera to send an image
					var hr = _vidControl.SetMode(_pinStill, VideoControlFlags.Trigger);
					DsError.ThrowExceptionForHR(hr);
				}

				var tcs = new TaskCompletionSource<object>();
				var registration = ThreadPool.RegisterWaitForSingleObject(_pictureReady, (state, timedOut) =>
				{
					var localTcs = (TaskCompletionSource<object>)state;
					if (timedOut)
						localTcs.TrySetCanceled();
					else
						localTcs.TrySetResult(null);
				}, tcs, 9000, executeOnlyOnce: true);
				await tcs.Task;
				var result = registration.Unregister(null);

				if (!result)
				{
					throw new Exception("Timeout waiting to get picture");
				}

				return BitmapSource.Create(this.Width, this.Height, 72, 72, PixelFormats.Bgr24, null, _ipBuffer, size, this.Stride);
			}
			finally
			{
				Marshal.FreeCoTaskMem(_ipBuffer);
			}

		}


		/// <summary> build the capture graph for grabber. </summary>
		private void SetupGraph(DsDevice dev, int width, int height, short depth, IntPtr handle)
		{
			ISampleGrabber sampGrabber = null;
			IPin pCaptureOut = null;
			IPin pSampleIn = null;
			IPin pRenderIn = null;

			// Get the graph builder object
			_filterGraph = (IFilterGraph2)new FilterGraph();

			try
			{
#if DEBUG
				_rot = new DsROTEntry(_filterGraph);
#endif
				// add the video input device
				var hr = _filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out IBaseFilter capFilter);
				DsError.ThrowExceptionForHR(hr);

				switch (this.Mode)
				{
					case CameraPerformanceMode.Normal:
						// Find the still pin
						_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Still, 0);
						if (_pinStill != null)
						{
							// Get a control pointer (used in Click())
							_vidControl = capFilter as IAMVideoControl;

							pCaptureOut = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);

							// If any of the default config items are set
							if (height + width + depth > 0)
							{
								this.SetConfigParams(_pinStill, width, height, depth);
							}
						}
						else
						{
							throw new NotSupportedException("still pin not found, consider use FastCapture mode");
						}
						break;
					case CameraPerformanceMode.FastCapture:
						_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Preview, 0);
						if (_pinStill == null)
						{
							IPin pRaw = null;
							IPin pSmart = null;

							// There is no still pin
							this._vidControl = null;

							// Add a splitter
							var iSmartTee = (IBaseFilter)new SmartTee();

							try
							{
								hr = this._filterGraph.AddFilter(iSmartTee, "SmartTee");
								DsError.ThrowExceptionForHR(hr);

								// Find the find the capture pin from the video device and the
								// input pin for the splitter, and connect them
								pRaw = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
								pSmart = DsFindPin.ByDirection(iSmartTee, PinDirection.Input, 0);

								hr = this._filterGraph.Connect(pRaw, pSmart);
								DsError.ThrowExceptionForHR(hr);

								// If any of the default config items are set, perform the config
								// on the actual video device (rather than the splitter)
								if (height + width + depth > 0)
								{
									this.SetConfigParams(pRaw, width, height, depth);
								}

								// Now set the capture and still pins (from the splitter)
								this._pinStill = DsFindPin.ByName(iSmartTee, "Preview");
								pCaptureOut = DsFindPin.ByName(iSmartTee, "Capture");
							}
							finally
							{
								if (pRaw != null)
								{
									Marshal.ReleaseComObject(pRaw);
								}
								if (pRaw != pSmart && pSmart != null)
								{
									Marshal.ReleaseComObject(pSmart);
								}
								if (pRaw != iSmartTee)
								{
									Marshal.ReleaseComObject(iSmartTee);
								}
							}
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				// Get the SampleGrabber interface
				sampGrabber = new SampleGrabber() as ISampleGrabber;

				// Configure the sample grabber
				var baseGrabFlt = sampGrabber as IBaseFilter;
				this.ConfigureSampleGrabber(sampGrabber);
				pSampleIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);

				// Get the default video renderer
				var pRenderer = new VideoRendererDefault() as IBaseFilter;
				hr = _filterGraph.AddFilter(pRenderer, "Renderer");
				DsError.ThrowExceptionForHR(hr);

				pRenderIn = DsFindPin.ByDirection(pRenderer, PinDirection.Input, 0);

				// Add the sample grabber to the graph
				hr = _filterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
				DsError.ThrowExceptionForHR(hr);

				if (_vidControl == null)
				{
					// Connect the Still pin to the sample grabber
					hr = _filterGraph.Connect(_pinStill, pSampleIn);
					DsError.ThrowExceptionForHR(hr);

					// Connect the capture pin to the renderer
					hr = _filterGraph.Connect(pCaptureOut, pRenderIn);
					DsError.ThrowExceptionForHR(hr);
				}
				else
				{
					// Connect the capture pin to the renderer
					hr = _filterGraph.Connect(pCaptureOut, pRenderIn);
					DsError.ThrowExceptionForHR(hr);

					// Connect the Still pin to the sample grabber
					hr = _filterGraph.Connect(_pinStill, pSampleIn);
					DsError.ThrowExceptionForHR(hr);
				}

				// Learn the video properties
				this.SaveSizeInfo(sampGrabber);
				this.ConfigVideoWindow(handle);

				// Start the graph
				var mediaCtrl = (IMediaControl)_filterGraph;
				hr = mediaCtrl.Run();
				DsError.ThrowExceptionForHR(hr);
			}
			finally
			{
				if (sampGrabber != null)
				{
					Marshal.ReleaseComObject(sampGrabber);
				}
				if (pCaptureOut != null)
				{
					Marshal.ReleaseComObject(pCaptureOut);
				}
				if (pRenderIn != null)
				{
					Marshal.ReleaseComObject(pRenderIn);
				}
				if (pSampleIn != null)
				{
					Marshal.ReleaseComObject(pSampleIn);
				}
			}
		}

		private void SaveSizeInfo(ISampleGrabber sampGrabber)
		{
			// Get the media type from the SampleGrabber
			var media = new AMMediaType();

			var hr = sampGrabber.GetConnectedMediaType(media);
			DsError.ThrowExceptionForHR(hr);

			if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
			{
				throw new NotSupportedException("Unknown Grabber Media Format");
			}

			// Grab the size info
			var videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
			this.Width = videoInfoHeader.BmiHeader.Width;
			this.Height = videoInfoHeader.BmiHeader.Height;
			this.Stride = this.Width * (videoInfoHeader.BmiHeader.BitCount / 8);

			DsUtils.FreeAMMediaType(media);
		}

		// Set the video window within the control specified by hControl
		private void ConfigVideoWindow(IntPtr handle)
		{
			var ivw = (IVideoWindow)_filterGraph;

			// Set the parent
			var hr = ivw.put_Owner(handle);
			DsError.ThrowExceptionForHR(hr);

			// Turn off captions, etc
			hr = ivw.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
			DsError.ThrowExceptionForHR(hr);

			// Yes, make it visible
			hr = ivw.put_Visible(OABool.True);
			DsError.ThrowExceptionForHR(hr);

			Win32API.RECT clientRect;
			Win32API.GetClientRect(handle, out clientRect);

			// Move to upper left corner
			hr = ivw.SetWindowPosition(0, 0, clientRect.Right, clientRect.Bottom);
			DsError.ThrowExceptionForHR(hr);
		}

		private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
		{
			var media = new AMMediaType
			{
				majorType = MediaType.Video,
				subType = MediaSubType.RGB24,
				formatType = FormatType.VideoInfo
			};

			var hr = sampGrabber.SetMediaType(media);
			DsError.ThrowExceptionForHR(hr);

			DsUtils.FreeAMMediaType(media);

			// Configure the sample grabber
			hr = sampGrabber.SetCallback(this, 1);
			DsError.ThrowExceptionForHR(hr);
		}

		// Set the Framerate, and video size
		private void SetConfigParams(IPin pin, int width, int height, short depth)
		{
			var videoStreamConfig = (IAMStreamConfig)pin;

			// Get the existing format block
			var hr = videoStreamConfig.GetFormat(out AMMediaType media);
			DsError.ThrowExceptionForHR(hr);

			try
			{
				// copy out the video info header
				var v = new VideoInfoHeader();
				Marshal.PtrToStructure(media.formatPtr, v);

				// if overriding the width, set the width
				if (width > 0)
				{
					v.BmiHeader.Width = width;
				}

				// if overriding the Height, set the Height
				if (height > 0)
				{
					v.BmiHeader.Height = height;
				}

				// if overriding the bits per pixel
				if (depth > 0)
				{
					v.BmiHeader.BitCount = depth;
				}

				// Copy the media structure back
				Marshal.StructureToPtr(v, media.formatPtr, false);

				// Set the new format
				hr = videoStreamConfig.SetFormat(media);
				DsError.ThrowExceptionForHR(hr);
			}
			finally
			{
				DsUtils.FreeAMMediaType(media);
			}
		}

		/// <summary> Shut down capture </summary>
		private void CloseInterfaces()
		{
			try
			{
				if (_filterGraph != null)
				{
					var mediaCtrl = (IMediaControl)_filterGraph;

					// Stop the graph
					mediaCtrl.Stop();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			if (_filterGraph != null)
			{
				Marshal.ReleaseComObject(_filterGraph);
				_filterGraph = null;
			}

			if (_vidControl != null)
			{
				Marshal.ReleaseComObject(_vidControl);
				_vidControl = null;
			}

			if (_pinStill != null)
			{
				Marshal.ReleaseComObject(_pinStill);
				_pinStill = null;
			}
		}

		/// <summary> sample callback, NOT USED. </summary>
		int ISampleGrabberCB.SampleCB(double sampleTime, IMediaSample pSample)
		{
			Marshal.ReleaseComObject(pSample);
			return 0;
		}

		/// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
		int ISampleGrabberCB.BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
		{
			// Note that we depend on only being called once per call to Click.  Otherwise
			// a second call can overwrite the previous image.
			Debug.Assert(bufferLen == Math.Abs(this.Stride) * this.Height, "Incorrect buffer length");

			if (!_wantOne)
				return 0;

			_wantOne = false;
			Debug.Assert(_ipBuffer != IntPtr.Zero, "Uninitialized buffer");

			// Save the buffer
			Win32API.CopyMemory(_ipBuffer, pBuffer, bufferLen);

			// Picture is ready.
			_pictureReady.Set();

			return 0;
		}

		public void UpdateWindowSize(int width, int height)
		{
			var ivw = (IVideoWindow)_filterGraph;
			ivw.put_Width(width);
			ivw.put_Height(height);
		}
	}
}
