#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Controls;

using System.Drawing;

/// <summary>Sequentially calls multiple <see cref="ISubItemPainter"/>s.</summary>
public class CompositeSubItemPainter : ISubItemPainter
{
	private readonly ISubItemPainter[] _painters;

	/// <summary>Initializes <see cref="CompositeSubItemPainter"/>.</summary>
	/// <param name="painters">Sequence of <see cref="ISubItemPainter"/>s.</param>
	public CompositeSubItemPainter(params ISubItemPainter[] painters)
	{
		Verify.Argument.IsNotNull(painters);

		_painters = painters;
	}

	/// <inheritdoc/>
	public bool TryMeasure(SubItemMeasureEventArgs measureEventArgs, out Size size)
	{
		Verify.Argument.IsNotNull(measureEventArgs);

		foreach(var painter in _painters)
		{
			if(painter.TryMeasure(measureEventArgs, out size)) return true;
		}
		size = Size.Empty;
		return false;
	}

	/// <inheritdoc/>
	public bool TryPaint(SubItemPaintEventArgs paintEventArgs)
	{
		Verify.Argument.IsNotNull(paintEventArgs);

		foreach(var painter in _painters)
		{
			if(painter.TryPaint(paintEventArgs)) return true;
		}
		return false;
	}
}
