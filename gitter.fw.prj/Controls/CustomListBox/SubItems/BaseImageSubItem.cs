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

namespace gitter.Framework.Controls;

using System;
using System.Drawing;

/// <summary>Base class for image subitems.</summary>
public abstract class BaseImageSubItem : CustomListBoxSubItem
{
	/// <summary>Create <see cref="BaseImageSubItem"/>.</summary>
	/// <param name="id">Subitem id.</param>
	public BaseImageSubItem(int id)
		: base(id)
	{
	}

	/// <summary>Subitem image.</summary>
	public abstract Image Image { get; set; }

	/// <summary>Subitem overlay image.</summary>
	public virtual Image OverlayImage { get { return null; } set { } }

	/// <summary>Paint event handler.</summary>
	/// <param name="paintEventArgs">Paint event args.</param>
	protected override void OnPaint(SubItemPaintEventArgs paintEventArgs)
		=> paintEventArgs.PaintImage(Image, OverlayImage);

	/// <summary>Measure event handler.</summary>
	/// <param name="measureEventArgs">Measure event args.</param>
	/// <returns>Subitem content size.</returns>
	protected override Size OnMeasure(SubItemMeasureEventArgs measureEventArgs)
		=> measureEventArgs.MeasureImage(Image);
}
