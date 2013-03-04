namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	using gitter.Native;

	/// <summary>Extension methods for <see cref="T:SystemWindows.Forms.ProgressBar"/> class.</summary>
	public static class ProgressBarExtensions
	{
		/// <summary>Sets the style of <see cref="T:SystemWindows.Forms.ProgressBar"/>.</summary>
		/// <param name="progressBar">Progress bar control.</param>
		/// <param name="style">Style.</param>
		/// <remarks>WinVista+ required.</remarks>
		public static void SetStyleEx(this ProgressBar progressBar, ProgressBarStyleEx style)
		{
			Verify.Argument.IsNotNull(progressBar, "progressBar");

			const uint TDM_SET_PROGRESS_BAR_STATE = 0x400 + 16;
			User32.SendMessage(progressBar.Handle, TDM_SET_PROGRESS_BAR_STATE, (IntPtr)style, (IntPtr)0);
		}
	}

	/// <summary>ProgressBar style.</summary>
	public enum ProgressBarStyleEx
	{
		/// <summary>Normal style.</summary>
		Normal = 1,
		/// <summary>Error (red).</summary>
		Error = 2,
		/// <summary>Paused (yellow).</summary>
		Paused = 3,
	}
}
