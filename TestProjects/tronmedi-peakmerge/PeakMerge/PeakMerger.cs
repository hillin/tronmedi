using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace PeakMerge
{
	class PeakMerger
	{
		public List<string> InputPaths { get; }
		public string OutputFile { get; }

		public PeakMerger(List<string> inputPaths, string outputFile)
		{
			this.InputPaths = inputPaths;
			this.OutputFile = outputFile;
		}

		public void Process()
		{
			var files = this.GetInputFiles();
			if (files.Count == 0)
			{
				Console.Error.WriteLine("No input file found");
				Environment.Exit(-1);
			}

			var bitmaps = files.Select(f => new Bitmap(f)).ToArray();
			var size = bitmaps[0].Size;

			if (bitmaps.Any(b => b.Size != size))
			{
				Console.Error.WriteLine("Not all input files have the same dimension");
				Environment.Exit(-1);
			}

			var outputBitmap = new Bitmap(size.Width, size.Height);
			var featureBitmap = new Bitmap(size.Width, size.Height);

			this.UnsafeProcess(bitmaps, outputBitmap, featureBitmap);

			outputBitmap.Save(this.OutputFile);
			featureBitmap.Save(Path.Combine(Path.GetDirectoryName(this.OutputFile), Path.GetFileNameWithoutExtension(this.OutputFile) + ".feature" + Path.GetExtension(this.OutputFile)));
		}

		private unsafe void UnsafeProcess(Bitmap[] inputImages, Bitmap outputImage, Bitmap featureBitmap)
		{
			var kernel = new[,]
			{
				{3, 4, 5, 4, 3},
				{4, 7, 10, 7, 4},
				{5, 10, 0, 10, 5},
				{4, 7, 10, 7, 4},
				{3, 4, 5, 4, 3 },
			};

			var kernelSize = kernel.GetLength(0);
			var kernelMargin = kernelSize / 2;

			var width = outputImage.Width;
			var height = outputImage.Height;
			var rect = new Rectangle(0, 0, width, height);
			var inputDatum = inputImages.Select(i => i.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)).ToArray();
			var outputData = outputImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
			var featureData = featureBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

			var stride = outputData.Stride;

			var inputRowHeadPtrs = new byte*[inputDatum.Length];
			var inputPixelPtrs = new byte*[inputDatum.Length];
			for (var i = 0; i < inputDatum.Length; ++i)
			{
				inputRowHeadPtrs[i] = (byte*)inputDatum[i].Scan0 + stride * kernelMargin;
			}

			var outputRowHeadPtr = (byte*)outputData.Scan0 + stride * kernelMargin;
			var featrueRowHeadPtr = (byte*)featureData.Scan0 + stride * kernelMargin;

			int CalculateContrast(int index)
			{
				var ptr = inputPixelPtrs[index];
				var contrast = 0;
				for (var component = 0; component < 3; ++component)
				{
					var value = *ptr;
					for (var y = 0; y < kernelSize; ++y)
					{
						for (var x = 0; x < kernelSize; ++x)
						{
							contrast += kernel[x, y] * Math.Abs(*(ptr + (x - kernelMargin) * 3 + (y - kernelMargin) * stride) - value);
						}
					}

					++ptr;
				}

				return Math.Abs(contrast);
			}

			for (var y = kernelMargin; y < height - kernelMargin; y++)
			{
				for (var i = 0; i < inputDatum.Length; ++i)
				{
					inputPixelPtrs[i] = inputRowHeadPtrs[i] + 3 * kernelMargin;
				}
				var outputPixelPtr = outputRowHeadPtr + 3 * kernelMargin;
				var featurePixelPtr = featrueRowHeadPtr + 3 * kernelMargin;

				for (var x = kernelMargin; x < width - kernelMargin; x++)
				{
					var maxContrast = 0;
					var maxContrastIndex = 0;
					for (var i = 0; i < inputDatum.Length; ++i)
					{
						var contrast = CalculateContrast(i);
						if (contrast > maxContrast)
						{
							maxContrast = contrast;
							maxContrastIndex = i;
						}
					}


					var featureColor = (byte)((double)maxContrastIndex / inputImages.Length * 256);

					for (var component = 0; component < 3; ++component)
					{
						*(outputPixelPtr + component) = *(inputPixelPtrs[maxContrastIndex] + component);
						*(featurePixelPtr + component) = featureColor;
					}


					for (var i = 0; i < inputDatum.Length; ++i)
					{
						inputPixelPtrs[i] += 3;
					}
					outputPixelPtr += 3;
					featurePixelPtr += 3;
				}

				for (var i = 0; i < inputDatum.Length; ++i)
				{
					inputRowHeadPtrs[i] += stride;
				}
				outputRowHeadPtr += stride;
				featrueRowHeadPtr += stride;
			}

			for (var index = 0; index < inputImages.Length; index++)
			{
				inputImages[index].UnlockBits(inputDatum[index]);
			}

			outputImage.UnlockBits(outputData);
			featureBitmap.UnlockBits(featureData);
		}

		private List<string> GetInputFiles()
		{
			var files = new List<string>();
			foreach (var path in this.InputPaths)
			{
				if (File.Exists(path))
				{
					files.Add(path);
					continue;
				}

				if (Directory.Exists(path))
				{
					files.AddRange(Directory.GetFiles(path));
					continue;
				}

				Console.Error.WriteLine($"Unknown input file or directory: {path}");
				Environment.Exit(-1);
			}

			return files;
		}
	}
}

