namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

	using gitter.Native;

	[ComImport]
	[Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEFAF")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ITaskbarList
	{
		#region ITaskbarList

		void HrInit();

		void AddTab(IntPtr hWnd);

		void DeleteTab(IntPtr hWnd);

		void ActivateTab(IntPtr hWnd);

		void SetActiveAlt(IntPtr hWnd);

		#endregion

		#region ITaskbarList2

		void MarkFullscreenWindow(IntPtr hWnd, bool fFullscreen);

		#endregion

		#region ITaskbarList3

		void SetProgressValue(IntPtr hWnd, ulong Completed, ulong Total);

		void SetProgressState(IntPtr hWnd, TbpFlag Flags);

		void RegisterTab(IntPtr hWndTab, IntPtr hWndMDI);

		void UnregisterTab(IntPtr hWndTab);

		void SetTabOrder(IntPtr hWndTab, IntPtr hwndInsertBefore);

		void SetTabActive(IntPtr hWndTab, IntPtr hWndMDI, uint dwReserved);
		
		void ThumbBarAddButtons(IntPtr hWnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] ThumbButton[] pButtons);

		void ThumbBarUpdateButtons(IntPtr hWnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)]ThumbButton[] pButtons);
		
		void ThumbBarSetImageList(IntPtr hWnd, IntPtr himl);
		
		void SetOverlayIcon(IntPtr hWnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)]string pszDescription);
		
		void SetThumbnailTooltip(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]string pszTip);

		void SetThumbnailClip(IntPtr hWnd, ref RECT prcClip);

		#endregion
	}

	[ComImport]
	[Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
	[ClassInterface(ClassInterfaceType.None)]
	internal class TaskbarList { }

	[Flags]
	public enum TbpFlag : int
	{
		NoProgress = 0x00,
		Indeterminate = 0x01,
		Normal = 0x02,
		Error = 0x04,
		Paused = 0x08
	}

	[Flags]
	public enum ThumbButtonMask : int
	{
		Bitmap = 0x01,
		Icon = 0x02,
		ToolTip = 0x04,
		Flags = 0x08
	}

	[Flags]
	public enum ThumbButtonFlags : int
	{
		Enabled = 0x00,
		Disabled = 0x01,
		DisMissonClick = 0x02,
		NoBackground = 0x04,
		Hidden = 0x08,
		NonInterActive = 0x10,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ThumbButton
	{
		public ThumbButtonMask dwMask;
		public int iID;
		public int iBitmap;
		public IntPtr hIcon;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szTip;
		public ThumbButtonFlags dwFlags;
	}
}
