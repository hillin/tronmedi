using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.Camera
{
	public struct CameraPropertyRange
	{
		public int Min { get; }
		public int Max { get; }
		public int Step { get; }
		public int DefaultValue { get; }
		public bool IsManual { get; }

		public CameraPropertyRange(int min, int max, int step, int defaultValue, bool isManual)
		{
			this.Min = min;
			this.Max = max;
			this.Step = step;
			this.DefaultValue = defaultValue;
			this.IsManual = isManual;
		}
	}
}
