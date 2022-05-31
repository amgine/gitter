#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework;

using System;
using System.Drawing;
using System.Windows.Forms;

public readonly record struct Dpi(int X, int Y) : IEquatable<Dpi>
{
	public static readonly Dpi Default = new(96);

	public static readonly Dpi System = GetSystemDpi();

	public static Dpi FromControl(Control control)
		=> new(control.DeviceDpi);

	private static Dpi GetSystemDpi()
	{
		switch(Environment.OSVersion.Platform)
		{
			case PlatformID.Win32NT:
				var hdc = Native.User32.GetDC(IntPtr.Zero);
				if(hdc != IntPtr.Zero)
				{
					try
					{
						var x = Native.Gdi32.GetDeviceCaps(hdc, Native.DeviceCaps.LOGPIXELSX);
						var y = Native.Gdi32.GetDeviceCaps(hdc, Native.DeviceCaps.LOGPIXELSY);

						return new Dpi(x, y);
					}
					finally
					{
						Native.Gdi32.DeleteDC(hdc);
					}
				}
				break;
		}
		return Dpi.Default;
	}

	public Dpi(int dpi) : this(dpi, dpi)
	{
	}

	public Dpi(Graphics graphics) : this((int)graphics.DpiX, (int)graphics.DpiY)
	{
	}

	public int GetValue(Orientation orientation)
		=> orientation switch
		{
			Orientation.Horizontal => X,
			Orientation.Vertical   => Y,
			_ => throw new ArgumentException($"Unknown orientation: {orientation}", nameof(orientation)),
		};
}
