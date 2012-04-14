namespace gitter
{
	using System;
	using System.Windows.Forms;

	using gitter.Framework;

	/// <summary>Extension methods for <see cref="T:SystemWindows.Forms.Button"/> class.</summary>
	public static class ButtonExtenstions
	{
		public static void ShowUACShield(this Button button)
		{
			if(button == null) throw new ArgumentNullException("button");
			if(Utility.IsOSVistaOrNewer)
			{
				NativeMethods.SendMessage(button.Handle, 0x1600 + 0x000C, IntPtr.Zero, (IntPtr)(-1));
			}
		}
			
		public static void HideUACShield(this Button button)
		{
			if(button == null) throw new ArgumentNullException("button");
			if(Utility.IsOSVistaOrNewer)
			{
				NativeMethods.SendMessage(button.Handle, 0x1600 + 0x000C, IntPtr.Zero, IntPtr.Zero);
			}
		}
	}
}
