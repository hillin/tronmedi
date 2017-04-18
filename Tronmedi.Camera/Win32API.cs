using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tronmedi.Camera
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	class Win32API
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left, Top, Right, Bottom;

			public RECT(int left, int top, int right, int bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}
		}

		[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
		public static extern void CopyMemory(IntPtr destination, IntPtr source, [MarshalAs(UnmanagedType.U4)] int length);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		/// <summary>
		/// Window Styles. 
		/// </summary>
		[Flags]
		public enum WindowStyles : uint
		{
			/// <summary>The window is a child window.</summary>
			WS_CHILD = 0x40000000,

			/// <summary>The window is initially visible.</summary>
			WS_VISIBLE = 0x10000000,
		}

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr CreateWindowEx(
			uint dwExStyle,
			string lpClassName,
			string lpWindowName,
			WindowStyles dwStyle,
			int x,
			int y,
			int nWidth,
			int nHeight,
			IntPtr hWndParent,
			IntPtr hMenu,
			IntPtr hInstance,
			IntPtr lpParam);


		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyWindow(IntPtr hWnd);

	}
}
