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

namespace gitter.Framework;

using System.Drawing;
using System.Windows.Forms;

public readonly struct DpiConverter
{
	private readonly Dpi _from;
	private readonly Dpi _to;

	public static readonly DpiConverter Identity = new(from: Dpi.Default, to: Dpi.Default);

	public static DpiConverter FromDefaultTo(Dpi dpi) => new(from: Dpi.Default, to: dpi);

	public DpiConverter(Dpi from, Dpi to)
	{
		_from = from;
		_to   = to;
	}

	public DpiConverter(Graphics graphics)
	{
		_from = Dpi.Default;
		_to   = new Dpi(graphics);
	}

	public DpiConverter(Control control)
	{
		_from = Dpi.Default;
		_to   = Dpi.FromControl(control);
	}

	public Dpi From => _from;

	public Dpi To => _to;

	static int Scale(int value, int from, int to)
		=> (value * to + from - 1) / from;

	static float Scale(float value, int from, int to)
		=> value * to / from;

	public float ConvertX(float value)
		=> Scale(value, _from.X, _to.X);

	public float ConvertY(float value)
		=> Scale(value, _from.Y, _to.Y);

	public int ConvertX(int value)
		=> Scale(value, _from.X, _to.X);

	public int ConvertY(int value)
		=> Scale(value, _from.Y, _to.Y);

	public Padding Convert(Padding padding)
		=> new(
			left:   ConvertX(padding.Left),
			top:    ConvertY(padding.Top),
			right:  ConvertX(padding.Right),
			bottom: ConvertY(padding.Bottom));

	public Point Convert(Point point)
		=> new(
			x: ConvertX(point.X),
			y: ConvertY(point.Y));

	public PointF Convert(PointF point)
		=> new(
			x: ConvertX(point.X),
			y: ConvertY(point.Y));

	public Size Convert(Size size)
		=> new(
			width:  ConvertX(size.Width),
			height: ConvertY(size.Height));

	public SizeF Convert(SizeF size)
		=> new(
			width:  ConvertX(size.Width),
			height: ConvertY(size.Height));

	public Rectangle Convert(Rectangle rectangle)
		=> new(
			x:      ConvertX(rectangle.X),
			y:      ConvertY(rectangle.Y),
			width:  ConvertX(rectangle.Width),
			height: ConvertY(rectangle.Height));

	public RectangleF Convert(RectangleF rectangle)
		=> new(
			x:      ConvertX(rectangle.X),
			y:      ConvertY(rectangle.Y),
			width:  ConvertX(rectangle.Width),
			height: ConvertY(rectangle.Height));
}
