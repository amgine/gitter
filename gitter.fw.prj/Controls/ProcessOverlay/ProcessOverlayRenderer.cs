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

namespace gitter.Framework.Controls
{
	using System.Drawing;

	public abstract class ProcessOverlayRenderer
	{
		protected static readonly StringFormat StringFormat = new()
		{
			Alignment = StringAlignment.Center,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
			FormatFlags = StringFormatFlags.FitBlackBox,
		};

		protected static readonly StringFormat TitleStringFormat = new(StringFormat.GenericTypographic)
		{
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Center,
			Trimming = StringTrimming.EllipsisCharacter,
			FormatFlags = StringFormatFlags.FitBlackBox,
		};

		public static readonly ProcessOverlayRenderer Default = new DefaultOverlayRenderer();

		public static readonly ProcessOverlayRenderer MSVS2012Dark = new MSVS2012OverlayRenderer(MSVS2012OverlayRenderer.DarkColors);

		public abstract void Paint(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds);

		public abstract void PaintMessage(ProcessOverlay processOverlay, Graphics graphics, Rectangle bounds, string status);
	}
}
