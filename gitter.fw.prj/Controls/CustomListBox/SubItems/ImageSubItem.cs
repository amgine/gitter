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

/// <summary>Subitem with image content.</summary>
public class ImageSubItem : BaseImageSubItem
{
	private Image? _image;
	private Image? _overlayImage;

	/// <summary>Create <see cref="ImageSubItem"/>.</summary>
	/// <param name="id">Subitem id.</param>
	/// <param name="image">Subitem image.</param>
	/// <param name="overlayImage">Subitem overlay image.</param>
	public ImageSubItem(int id, Image? image, Image? overlayImage)
		: base(id)
	{
		_image = image;
		_overlayImage = overlayImage;
	}

	/// <summary>Create <see cref="ImageSubItem"/>.</summary>
	/// <param name="id">Subitem id.</param>
	/// <param name="image">Subitem image.</param>
	public ImageSubItem(int id, Image? image)
		: this(id, image, null)
	{
	}

	/// <summary>Create <see cref="ImageSubItem"/>.</summary>
	/// <param name="id">Subitem id.</param>
	public ImageSubItem(int id)
		: this(id, null, null)
	{
	}

	/// <summary>Subitem image.</summary>
	public override Image? Image
	{
		get => _image;
		set
		{
			if(_image == value) return;

			_image = value;
			Invalidate();
		}
	}

	/// <summary>Subitem overlay image.</summary>
	public override Image? OverlayImage
	{
		get => _overlayImage;
		set
		{
			if(_overlayImage == null) return;

			_overlayImage = value;
			Invalidate();
		}
	}
}
