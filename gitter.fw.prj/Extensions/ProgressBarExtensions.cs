#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
			Verify.Argument.IsNotNull(progressBar, nameof(progressBar));

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
