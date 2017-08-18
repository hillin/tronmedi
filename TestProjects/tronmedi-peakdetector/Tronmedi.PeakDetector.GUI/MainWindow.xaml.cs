using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;

namespace Tronmedi.PeakDetector
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			var folderDialog = new VistaFolderBrowserDialog()
			{
				ShowNewFolderButton = false,
				Description = "Open Images"
			};

			if (folderDialog.ShowDialog(this) != true)
			{
				Environment.Exit(-1);
			}

			this.BeginLoadImages(folderDialog.SelectedPath);
		}

		private void BeginLoadImages(string path)
		{
			new ImageFolderLoader(path).Load();
		}
	}
}
