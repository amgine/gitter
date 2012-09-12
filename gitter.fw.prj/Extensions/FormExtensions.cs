namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	/// <summary>Extension methods for <see cref="System.Windows.Forms.Form"/>.</summary>
	public static class FormExtensions
	{
		private const int SC_MOVE = 0xf010;
		private const int SC_MINIMIZE = 0xf020;
		private const int SC_MAXIMIZE = 0xf030;
		private const int SC_CLOSE = 0xf060;
		private const int SC_RESTORE = 0xf120;

		private const int MF_BYCOMMAND = 0x00000000;
		private const int MF_BYPOSITION = 0x00000400;
		private const int MF_DISABLED = 0x00000002;
		private const int MF_ENABLED = 0x00000000;
		private const int MF_GRAYED = 0x00000001;

		public static void DisableCloseButton(this Form form)
		{
			Verify.Argument.IsNotNull(form, "form");

			var hMenu = NativeMethods.GetSystemMenu(form.Handle, false);
			NativeMethods.EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
		}

		public static void EnableCloseButton(this Form form)
		{
			Verify.Argument.IsNotNull(form, "form");

			var hMenu = NativeMethods.GetSystemMenu(form.Handle, false);
			NativeMethods.EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
		}
	}
}
