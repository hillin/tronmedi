using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.Camera
{
	public struct CameraPropertyValue
	{
		public int Value { get; }
		public bool IsManual { get; }

		public CameraPropertyValue(int value, bool isManual)
		{
			this.Value = value;
			this.IsManual = isManual;
		}
	}
}
