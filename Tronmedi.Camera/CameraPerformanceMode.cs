using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tronmedi.Camera
{
	public enum CameraPerformanceMode
	{
		/// <summary>
		/// Use still pin to capture slow but high quality still images
		/// </summary>
		Normal,
		/// <summary>
		/// Use a splitter to grab still image directly from the preview pin. This could drastically reduce video framerate, but can capture still images quickly.
		/// </summary>
		FastCapture
	}
}
