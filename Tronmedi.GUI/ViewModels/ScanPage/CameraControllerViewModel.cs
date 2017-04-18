using System;
using System.Windows.Threading;
using Caliburn.Micro;
using Tronmedi.Camera;

namespace Tronmedi.GUI.ViewModels.ScanPage
{
	class CameraControllerViewModel : PropertyChangedBase
	{
		public int MinimumExposure
		{
			get => _minimumExposure;
			private set
			{
				if (value == _minimumExposure) return;
				_minimumExposure = value;
				this.NotifyOfPropertyChange(nameof(this.MinimumExposure));
			}
		}

		public int MaximumExposure
		{
			get => _maximumExposure;
			private set
			{
				if (value == _maximumExposure) return;
				_maximumExposure = value;
				this.NotifyOfPropertyChange(nameof(this.MaximumExposure));
			}
		}

		public int ExposureValue
		{
			get => _exposureValue;
			set
			{
				if (value == _exposureValue) return;
				_exposureValue = value;
				_isAutoExposure = false;
				this.NotifyOfPropertyChange(nameof(this.ExposureText));
				this.SetExposure();
			}
		}

		public int ExposureInterval
		{
			get => _exposureInterval;
			private set
			{
				if (value == _exposureInterval) return;
				_exposureInterval = value;
				this.NotifyOfPropertyChange(nameof(this.ExposureInterval));
			}
		}

		public bool IsSettingExposure
		{
			get => _isSettingExposure;
			private set
			{
				if (value == _isSettingExposure) return;
				_isSettingExposure = value;
				this.NotifyOfPropertyChange(nameof(this.IsSettingExposure));
			}
		}


		public bool IsAutoExposure
		{
			get => _isAutoExposure;
			set
			{
				if (value == _isAutoExposure) return;
				_isAutoExposure = value;
				this.NotifyOfPropertyChange(nameof(this.ExposureText));
				this.SetExposure();
			}
		}

		public string ExposureText
		{
			get
			{
				if (this.IsAutoExposure)
					return "Auto";

				return this.ExposureValue >= 0
					? $"{Math.Pow(2, this.ExposureValue)}\""
					: $"1/{Math.Pow(2, -this.ExposureValue)}";
			}
		}

		private readonly CameraControl _cameraControl;
		private int _minimumExposure;
		private int _maximumExposure;
		private int _exposureValue;
		private int _exposureInterval;
		private bool _isAutoExposure;

		private readonly DispatcherTimer _delaySetExposureTimer;
		private bool _isSettingExposure;

		public CameraControllerViewModel(CameraControl cameraControl)
		{
			_cameraControl = cameraControl;
			this.UpdateExposure();

			_delaySetExposureTimer = new DispatcherTimer(DispatcherPriority.Normal)
			{
				Interval = TimeSpan.FromSeconds(0.5)
			};
			_delaySetExposureTimer.Tick += this.DelaySetExposureTimer_Tick;
		}

		private void DelaySetExposureTimer_Tick(object sender, EventArgs e)
		{
			this.IsSettingExposure = true;
			_delaySetExposureTimer.Stop();

			_cameraControl.SetProperty(DirectShowLib.CameraControlProperty.Exposure,
				new CameraPropertyValue(this.ExposureValue, !this.IsAutoExposure));

			this.UpdateExposure();

			this.IsSettingExposure = false;
		}

		private void UpdateExposure()
		{
			var range = _cameraControl.GetPropertyRange(DirectShowLib.CameraControlProperty.Exposure);
			this.MinimumExposure = range.Min;
			this.MaximumExposure = range.Max;
			this.ExposureInterval = range.Step;

			var value = _cameraControl.GetProperty(DirectShowLib.CameraControlProperty.Exposure);
			_isAutoExposure = !value.IsManual;
			_exposureValue = value.Value;
			this.NotifyOfPropertyChange(nameof(this.ExposureValue));
			this.NotifyOfPropertyChange(nameof(this.IsAutoExposure));
			this.NotifyOfPropertyChange(nameof(this.ExposureText));
		}

		private void SetExposure()
		{
			_delaySetExposureTimer.Stop();
			_delaySetExposureTimer.Start();
		}
	}
}
