using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace Tronmedi.PeakFocus
{
	public class ImagePeakInfo
	{
		public ImagePeakInfo(string path)
		{
			var origin = new Mat(path, ImreadModes.Color);
			//mat = mat.GaussianBlur(new Size(9, 9), 0);
			var mat = origin.CvtColor(ColorConversionCodes.BGR2GRAY);
			//mat = mat.Laplacian(MatType.CV_64F);

			mat = mat.Resize(new Size(400, 300));

			var kernelData = new[,]
			{
				{1, 1, 1},
				{1, -8, 1},
				{1, 1, 1}
			};
			var kernel = InputArray.Create(kernelData);
			mat = mat.Filter2D(MatType.CV_32F, kernel);
			mat = mat.BilateralFilter(21, 75, 75);

			mat.ConvertTo(mat, MatType.CV_8U, Math.Pow(2, 24));
			mat = mat.CvtColor(ColorConversionCodes.GRAY2BGR);
			//mat = mat.GaussianBlur(new Size(25,25), 0);

			//var matOfByte = new MatOfByte(mat);
			//var indexer = matOfByte.GetIndexer();
			//for (var y = 0; y < mat.Height; y++)
			//{
			//	for (var x = 0; x < mat.Width; x++)
			//	{
			//		var pixel = indexer[y, x];

			//		indexer[y, x] = pixel;
			//	}
			//}

			mat = mat.Resize(origin.Size());

			var output = origin.Clone();
			Cv2.AddWeighted(origin, .5f, mat, .5f, 0, output);

			output = output.Resize(new Size(1024, 768));

			//Cv2.ImShow("Origin", origin);
			//Cv2.ImShow("Mat", mat);
			Cv2.ImShow("Output", output);
			Cv2.WaitKey();
			Cv2.DestroyAllWindows();

		}
	}
}
