namespace gitter.Framework
{
	using System;
	using System.Security;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	using System.Drawing;

	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		public static extern int SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)]string appID);

		[Serializable]
		[StructLayout(LayoutKind.Sequential)]
		public struct SHELLEXECUTEINFO
		{
			public int cbSize;
			public uint fMask;
			public IntPtr hwnd;
			public string lpVerb;
			public string lpFile;
			public string lpParameters;
			public string lpDirectory;
			public uint nShow;
			public IntPtr hInstApp;
			public IntPtr lpIDList;
			public string lpClass;
			public IntPtr hkeyClass;
			public uint dwHotKey;
			public IntPtr hIcon;
			public IntPtr hProcess;
		}

		[DllImport("shell32.dll", SetLastError = true)]
		public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

		[DllImport("gdi32.dll")]
		public static extern int SetDIBitsToDevice(
			  IntPtr hdc,
			  int XDest,
			  int YDest,
			  int dwWidth,
			  int dwHeight,
			  int XSrc,
			  int YSrc,
			  int uStartScan,
			  int cScanLines,
			  IntPtr lpvBits,
			  ref BITMAPINFOHEADER lpbmi,
			  int fuColorUse
			);

		[StructLayout(LayoutKind.Sequential)]
		public struct BITMAPINFOHEADER
		{
			public int biSize;
			public int biWidth;
			public int biHeight;
			public short biPlanes;
			public short biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
		}

		[DllImport("shell32.dll")]
		public static extern IntPtr ExtractAssociatedIcon(
			IntPtr hInst,
			string lpIconPath,
			ref short lpiIcon
		);

		[DllImport("shell32.dll")]
		public static extern int ExtractIconEx(
			string lpszFile,
			int nIconIndex,
			IntPtr[] phiconLarge,
			IntPtr[] phiconSmall,
			int nIcons
		);

		[DllImport("user.dll")]
		public static extern bool DestroyIcon(
			IntPtr hIcon
		);

		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public int dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=256)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=80)]
			public string szTypeName;
		}

		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(
			string pszPath,
			int dwFileAttributes,
			ref SHFILEINFO psfi,
			int cbFileInfo,
			int uFlags
		);

		public const int WS_EX_NOACTIVATE = 0x08000000;
		public const int HTTRANSPARENT = -1;

		public const int HTCAPTION = 2;
		public const int HTLEFT = 10;
		public const int HTRIGHT = 11;
		public const int HTTOP = 12;
		public const int HTTOPLEFT = 13;
		public const int HTTOPRIGHT = 14;
		public const int HTBOTTOM = 15;
		public const int HTBOTTOMLEFT = 16;
		public const int HTBOTTOMRIGHT = 17;

		public const int CBN_SELCHANGE = 1;
		public const int CBN_DROPDOWN = 7;
		public const int CBN_CLOSEUP = 8;
		public const int CBN_SELENDOK = 9;
		public const int CBN_SELENDCANCEL = 10;

		[DllImport("kernel32.dll")]
		public static extern void MoveMemory(IntPtr dest, IntPtr src, UIntPtr len);

		[DllImport("kernel32.dll")]
		public static extern void CopyMemory(IntPtr dest, IntPtr src, UIntPtr len);

		//[DllImport("gdi32.dll")]
		//public static extern int SetDIBitsToDevice(
		//      IntPtr hdc,
		//      int XDest,
		//      int YDest,
		//      int dwWidth,
		//      int dwHeight,
		//      int XSrc,
		//      int YSrc,
		//      int uStartScan,
		//      int cScanLines,
		//      IntPtr lpvBits,
		//      ref BITMAPINFOHEADER lpbmi,
		//      int fuColorUse
		//    );

		[DllImport("user32.dll")]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool revert);

		[DllImport("user32.dll")]
		public static extern int EnableMenuItem(IntPtr hMenu, int IDEnableItem, int wEnable);

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern int RegisterWindowMessage(string lpString);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(Point lpPoint);

		public const int GA_ROOT = 2;

		[DllImport("user32.dll")]
		public static extern IntPtr GetAncestor(IntPtr hwnd, int gaFlags);


		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr GetWindow(IntPtr hwnd, int wFlag);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int AnimateWindow(HandleRef windowHandle, int time, AnimationFlags flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, WindowsMessage msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostMessage(IntPtr hWnd, WindowsMessage msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr handle, int flags);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(
			IntPtr hWnd,
			IntPtr hWndInsertAfter,
			int X,
			int Y,
			int cx,
			int cy,
			int uFlags);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool LockWindowUpdate(IntPtr hWndLock);


		public delegate IntPtr WNDPROC(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		public const int GWLP_WNDPROC = -4;

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WNDPROC dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, WNDPROC dwNewLong);

		[Flags]
		public enum RedrawWindowFlags : uint
		{
			/// <summary>
			/// Invalidates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
			/// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_INVALIDATE invalidates the entire window.
			/// </summary>
			Invalidate = 0x1,

			/// <summary>Causes the OS to post a WM_PAINT message to the window regardless of whether a portion of the window is invalid.</summary>
			InternalPaint = 0x2,

			/// <summary>
			/// Causes the window to receive a WM_ERASEBKGND message when the window is repainted.
			/// Specify this value in combination with the RDW_INVALIDATE value; otherwise, RDW_ERASE has no effect.
			/// </summary>
			Erase = 0x4,

			/// <summary>
			/// Validates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
			/// You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_VALIDATE validates the entire window.
			/// This value does not affect internal WM_PAINT messages.
			/// </summary>
			Validate = 0x8,

			NoInternalPaint = 0x10,

			/// <summary>Suppresses any pending WM_ERASEBKGND messages.</summary>
			NoErase = 0x20,

			/// <summary>Excludes child windows, if any, from the repainting operation.</summary>
			NoChildren = 0x40,

			/// <summary>Includes child windows, if any, in the repainting operation.</summary>
			AllChildren = 0x80,

			/// <summary>Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND and WM_PAINT messages before the RedrawWindow returns, if necessary.</summary>
			UpdateNow = 0x100,

			/// <summary>
			/// Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive WM_ERASEBKGND messages before RedrawWindow returns, if necessary.
			/// The affected windows receive WM_PAINT messages at the ordinary time.
			/// </summary>
			EraseNow = 0x200,

			Frame = 0x400,

			NoFrame = 0x800
		}

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool RedrawWindow(IntPtr hWnd, ref RECT lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr GetDC(
			[In] IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern int ReleaseDC(
			[In] IntPtr hWnd,
			[In] IntPtr hDC);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool ScrollDC(
			[In] IntPtr hDC,
			[In] int dx,
			[In] int dy,
			[In] ref RECT lprcScroll,
			[In] ref RECT lprcClip,
			[In] IntPtr hrgnUpdate,
			[Out]out RECT lprcUpdate);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		public static extern int ScrollWindowEx(
			[In]  IntPtr hWnd,
			[In]  int nXAmount,
			[In]  int nYAmount,
			[In]  ref RECT rectScrollRegion,
			[In]  ref RECT rectClip,
			[In]  IntPtr hrgnUpdate,
			[Out] out RECT prcUpdate,
			[In]  int flags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
		public static extern int ScrollWindowEx(
			[In]  IntPtr hWnd,
			[In]  int nXAmount,
			[In]  int nYAmount,
			[In]  ref RECT rectScrollRegion,
			[In]  ref RECT rectClip,
			[In]  IntPtr hrgnUpdate,
			[In]  IntPtr prcUpdate,
			[In]  int flags);

		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public RECT(int l, int t, int r, int b)
			{
				left = l;
				top = t;
				right = r;
				bottom = b;
			}

			public RECT(Rectangle rect)
			{
				left = rect.Left;
				top = rect.Top;
				right = rect.Right;
				bottom = rect.Bottom;
			}
		};

		public struct COMBOBOXINFO
		{
			public uint cbSize;
			public RECT rcItem;
			public RECT rcButton;
			public uint stateButton;
			public IntPtr hwndCombo;
			public IntPtr hwndItem;
			public IntPtr hwndList;
		};

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref COMBOBOXINFO pcbi);

		public const int CB_SHOWDROPDOWN = 0x014F;
		public const int CB_SELECTSTRING = 0x014D;

		public static void ShowDropDown(IntPtr handle, bool show)
		{
			SendMessage(handle, CB_SHOWDROPDOWN, (IntPtr)(show ? 1 : 0), IntPtr.Zero);
		}

		//public struct BITMAPINFOHEADER
		//{
		//    public int biSize;
		//    public int biWidth;
		//    public int biHeight;
		//    public short biPlanes;
		//    public short biBitCount;
		//    public int biCompression;
		//    public int biSizeImage;
		//    public int biXPelsPerMeter;
		//    public int biYPelsPerMeter;
		//    public int biClrUsed;
		//    public int biClrImportant;
		//}

		[Flags]
		public enum AnimationFlags : int
		{
			Roll = 0x0000, // Uses a roll animation.
			HorizontalPositive = 0x00001, // Animates the window from left to right. This flag can be used with roll or slide animation.
			HorizontalNegative = 0x00002, // Animates the window from right to left. This flag can be used with roll or slide animation.
			VerticalPositive = 0x00004, // Animates the window from top to bottom. This flag can be used with roll or slide animation.
			VerticalNegative = 0x00008, // Animates the window from bottom to top. This flag can be used with roll or slide animation.
			Center = 0x00010, // Makes the window appear to collapse inward if <c>Hide</c> is used or expand outward if the <c>Hide</c> is not used.
			Hide = 0x10000, // Hides the window. By default, the window is shown.
			Activate = 0x20000, // Activates the window.
			Slide = 0x40000, // Uses a slide animation. By default, roll animation is used.
			Blend = 0x80000, // Uses a fade effect. This flag can be used only with a top-level window.
			Mask = 0xfffff,
		}

		public static void AnimateWindow(Control control, int time, AnimationFlags flags)
		{
			AnimateWindow(new HandleRef(control, control.Handle), time, flags);
		}

		public static int HIWORD(int n)
		{
			return (n >> 16) & 0xffff;
		}

		public static int HIWORD(IntPtr n)
		{
			return HIWORD(unchecked((int)(long)n));
		}

		public static int LOWORD(int n)
		{
			return n & 0xffff;
		}

		public static int LOWORD(IntPtr n)
		{
			return LOWORD(unchecked((int)(long)n));
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MINMAXINFO
		{
			public Point reserved;
			public Size maxSize;
			public Point maxPosition;
			public Size minTrackSize;
			public Size maxTrackSize;
		}
	}
}
