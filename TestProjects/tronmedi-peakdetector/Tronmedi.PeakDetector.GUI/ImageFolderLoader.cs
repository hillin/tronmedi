using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ookii.Dialogs.Wpf;
using Tronmedi.PeakFocus;

namespace Tronmedi.PeakDetector
{
	internal class ImageFolderLoader
	{
		private readonly ProgressDialog _progressDialog;
		public string Path { get; }

		public ImageFolderLoader(string path)
		{
			this.Path = path;

			_progressDialog = new ProgressDialog
			{
				WindowTitle = "Loading",
			};

			_progressDialog.DoWork += ProgressDialog_DoWork;
		}

		public void Load()
		{

			new ImagePeakInfo(Directory.GetFiles(this.Path, "*.jpg")[0]);
			new ImagePeakInfo(Directory.GetFiles(this.Path, "*.jpg")[17]);
			_progressDialog.ShowDialog();
		}

		private void ProgressDialog_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			_progressDialog.ProgressBarStyle = ProgressBarStyle.MarqueeProgressBar;

			var files = Directory.GetFiles(this.Path, "*.jpg");

			_progressDialog.ProgressBarStyle = ProgressBarStyle.ProgressBar;
			_progressDialog.ReportProgress(0, null, "Preparing...");

			for (var i = 0; i < files.Length; ++i)
			{
				var file = files[i];
				_progressDialog.ReportProgress(i * 100 / files.Length, null, $"Loading {file} ({i + 1}/{files.Length})...");
			}
			
			_progressDialog.ReportProgress(0, "Complete", "Complete");
		}
	}
}
