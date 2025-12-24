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

namespace gitter.Git.Gui.Controls;

using System.Globalization;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary>Paints <see cref="FakeRevisionListItem.SubjectText"/> for the <see cref="SubjectColumn"/>.</summary>
sealed class FakeRevisionListItemSubjectPainter : ISubItemPainter
{
	public bool TryMeasure(SubItemMeasureEventArgs measureEventArgs, out Size size)
	{
		Verify.Argument.IsNotNull(measureEventArgs);

		if( measureEventArgs.Item   is not FakeRevisionListItem item ||
			measureEventArgs.Column is not SubjectColumn)
		{
			size = Size.Empty;
			return false;
		}

		size = measureEventArgs.MeasureText(item.SubjectText);
		return true;
	}

	public bool TryPaint(SubItemPaintEventArgs paintEventArgs)
	{
		Verify.Argument.IsNotNull(paintEventArgs);

		if(paintEventArgs.Item   is not FakeRevisionListItem item) return false;
		if(paintEventArgs.Column is not SubjectColumn)             return false;

		var rect = paintEventArgs.Bounds;
		var conv = paintEventArgs.DpiConverter;
		var alignToGraph = paintEventArgs.Column is SubjectColumn rsc
			? rsc.AlignToGraph
			: SubjectColumn.DefaultAlignToGraph;
		var graphColumn = paintEventArgs.ListBox.GetPrevVisibleColumn(paintEventArgs.ColumnIndex);
		if(graphColumn is { Id: not (int)ColumnId.Graph })
		{
			graphColumn = null;
		}
		if(alignToGraph && graphColumn is not null)
		{
			int availWidth;
			if(item.Graph is not null)
			{
				availWidth = graphColumn.CurrentWidth - paintEventArgs.ListBox.CurrentItemHeight * item.Graph.Length;
				for(int i = item.Graph.Length - 1; i != -1; --i)
				{
					if(!item.Graph[i].IsEmpty)
					{
						break;
					}
					availWidth += paintEventArgs.ListBox.CurrentItemHeight;
				}
			}
			else
			{
				availWidth = graphColumn.CurrentWidth;
			}
			if(availWidth != 0)
			{
				rect.X     -= availWidth;
				rect.Width += availWidth;
			}
		}
		paintEventArgs.PrepareContentRectangle(ref rect);
		if(rect.Width > 1)
		{
			var text = item.SubjectText;
			paintEventArgs.PrepareTextRectangle(ref rect);
			var useDefaultBrush = (paintEventArgs.State & ItemState.Selected) == ItemState.Selected && paintEventArgs.ListBox.Style.Type == GitterStyleType.DarkBackground;
			var textBrush = useDefaultBrush ? paintEventArgs.Brush : new SolidBrush(paintEventArgs.ListBox.Style.Colors.GrayText);
			if(!string.IsNullOrWhiteSpace(text))
			{
				var w = GitterApplication.TextRenderer.MeasureText(
					paintEventArgs.Graphics, text, paintEventArgs.Font, int.MaxValue).Width;
					GitterApplication.TextRenderer.DrawText(
						paintEventArgs.Graphics, text, paintEventArgs.Font, textBrush, rect);
				w += conv.ConvertX(3);
				rect.X     += w;
				rect.Width -= w;
			}
			var iconSize = conv.Convert(new Size(16, 16));
			for(int i = 0; i < item.Icons.Length; ++i)
			{
				if(rect.Width <= 0) break;
				if(item.Icons[i].Count != 0)
				{
					var image = item.Icons[i].Image.GetImage(paintEventArgs.Dpi.X * 16 / 96);
					if(image is not null)
					{
						var imageRect = new Rectangle(rect.X, rect.Y - 1 + (rect.Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
						var dx = imageRect.Width + conv.ConvertX(2);
						rect.X     += dx;
						rect.Width -= dx;
						if(rect.Width <= 0) break;
						paintEventArgs.Graphics.DrawImage(image, imageRect);
					}
					var countText = item.Icons[i].Count.ToString(CultureInfo.CurrentCulture);
					var textW = GitterApplication.TextRenderer.MeasureText(
						paintEventArgs.Graphics, countText, paintEventArgs.Font, int.MaxValue).Width;
					GitterApplication.TextRenderer.DrawText(
						paintEventArgs.Graphics, countText, paintEventArgs.Font, textBrush, rect);
					textW += conv.ConvertX(2);
					rect.X     += textW;
					rect.Width -= textW;
				}
			}
			if(!useDefaultBrush)
			{
				textBrush.Dispose();
			}
		}

		return true;
	}
}
