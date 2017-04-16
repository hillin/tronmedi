using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Tronmedi.GUI.Helpers
{
	public class AirspaceOverlay : Decorator
	{
		private readonly Window _transparentInputWindow;
		private Window _parent;

		public AirspaceOverlay()
		{
			_transparentInputWindow = new Window
			{
				Background = Brushes.Transparent,   //Make the window itself transparent, with no style
				AllowsTransparency = true,
				WindowStyle = WindowStyle.None,
				ShowInTaskbar = false,  //Hide from taskbar until it becomes a child
				Focusable = false //HACK: This window and it's child controls should never have focus, as window styling of an invisible window 
								  //will confuse user.
			};
			
			_transparentInputWindow.PreviewMouseDown += transparentInputWindow_PreviewMouseDown;
		}

		private void transparentInputWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			_parent.Focus();
		}

		public object OverlayChild
		{
			get
			{
				return _transparentInputWindow?.Content;
			}
			set
			{
				if (_transparentInputWindow != null)
				{
					_transparentInputWindow.Content = value;
				}
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			UpdateWindow();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			if (_transparentInputWindow.Visibility != Visibility.Visible)
			{
				UpdateWindow();
				_transparentInputWindow.Show();
				_parent = GetParentWindow(this);
				_transparentInputWindow.Owner = _parent;
				_parent.LocationChanged += parent_LocationChanged;
			}
		}

		private void parent_LocationChanged(object sender, EventArgs e)
		{
			UpdateWindow();
		}

		private static Window GetParentWindow(DependencyObject o)
		{
			var parent = VisualTreeHelper.GetParent(o);
			if (parent == null)
			{
				var fe = o as FrameworkElement;
				if (fe != null)
				{
					if (fe is Window)
					{
						return fe as Window;
					}
					if (fe.Parent != null)
					{
						return GetParentWindow(fe.Parent);
					}
				}
				throw new ApplicationException("A window parent could not be found for " + o);
			}
			return GetParentWindow(parent);
		}

		private void UpdateWindow()
		{
			var hostTopLeft = PointToScreen(new Point(0, 0));
			var transformToDevice = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;

			_transparentInputWindow.Left = hostTopLeft.X / transformToDevice.M11;
			_transparentInputWindow.Top = hostTopLeft.Y / transformToDevice.M22;
			_transparentInputWindow.Width = ActualWidth;
			_transparentInputWindow.Height = ActualHeight;
		}
		
	}
}
