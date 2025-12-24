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

using System;
using System.Resources;
using System.Drawing;
using System.Drawing.Drawing2D;

/// <summary>Provides cached bitmap resources.</summary>
public sealed class CachedBitmapResources(ResourceManager resourceManager)
	: CachedResources<Bitmap>(resourceManager)
{
	public Bitmap? CombineBitmaps(string resBackground, string resOverlay)
	{
		var background = this[resBackground];
		if(background is null) return default;
		var ovl = this[resOverlay];
		if(ovl is null) return background;
		var res = new Bitmap(background);
		using(var graphics = Graphics.FromImage(res))
		{
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			var rc = new Rectangle(0, 0, ovl.Width, ovl.Height);
			graphics.DrawImage(ovl, rc, rc, GraphicsUnit.Pixel);
		}
		return res;
	}
}
