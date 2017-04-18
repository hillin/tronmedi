using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Tronmedi.Camera
{
	internal class VideoWindow : HwndHost
	{
		public new event EventHandler<Size> SizeChanged;
		internal new IntPtr Handle { get; private set; } = IntPtr.Zero;

		protected override void OnWindowPositionChanged(Rect rcBoundingBox)
		{
			base.OnWindowPositionChanged(rcBoundingBox);
			this.SizeChanged?.Invoke(this, new Size(rcBoundingBox.Width, rcBoundingBox.Height));
		}

		/// <summary>
		/// Creates the window to be hosted. 
		/// </summary>
		/// <returns>
		/// The handle to the child Win32 window to create.
		/// </returns>
		/// <param name="hWndParent">The window handle of the parent window.</param>
		protected override HandleRef BuildWindowCore(HandleRef hWndParent)
		{
			Win32API.RECT clientArea;
			Win32API.GetClientRect(hWndParent.Handle, out clientArea);

			this.Handle = Win32API.CreateWindowEx(0, "Static", "", Win32API.WindowStyles.WS_CHILD | Win32API.WindowStyles.WS_VISIBLE,
							0, 0, clientArea.Right - clientArea.Left, clientArea.Bottom - clientArea.Top,
							hWndParent.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			return new HandleRef(this, this.Handle);
		}

		/// <summary>
		/// Destroys the hosted window. 
		/// </summary>
		/// <param name="hWnd">A structure that contains the window handle.</param>
		protected override void DestroyWindowCore(HandleRef hWnd)
		{
			Win32API.DestroyWindow(hWnd.Handle);
		}


	}
}
