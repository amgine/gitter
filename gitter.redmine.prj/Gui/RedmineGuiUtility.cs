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

namespace gitter.Redmine.Gui
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Redmine.Properties.Resources;

	internal static class RedmineGuiUtility
	{
		private static void PaintGrayText(string text, SubItemPaintEventArgs paintEventArgs)
		{
			var style = paintEventArgs.ListBox.Style;
			if((paintEventArgs.State & ItemState.Selected) == ItemState.Selected && style.Type == GitterStyleType.DarkBackground)
			{
				paintEventArgs.PaintText(text, paintEventArgs.Brush);
			}
			else
			{
				using(var brush = new SolidBrush(style.Colors.GrayText))
				{
					paintEventArgs.PaintText(text, brush);
				}
			}
		}

		public static void PaintOptionalContent(NamedRedmineObject data, SubItemPaintEventArgs paintEventArgs)
		{
			if(data == null)
			{
				PaintGrayText(Resources.StrsUnassigned.SurroundWith('<', '>'), paintEventArgs);
			}
			else
			{
				paintEventArgs.PaintText(data.Name, paintEventArgs.Brush);
			}
		}

		public static void PaintOptionalContent(DateTime? date, SubItemPaintEventArgs paintEventArgs)
		{
			if(!date.HasValue)
			{
				PaintGrayText(Resources.StrsUnassigned.SurroundWith('<', '>'), paintEventArgs);
			}
			else
			{
				DateColumn.OnPaintSubItem(paintEventArgs, date.Value);
			}
		}

		public static void PaintOptionalContent(string data, SubItemPaintEventArgs paintEventArgs)
		{
			if(data == null)
			{
				PaintGrayText(Resources.StrsUnassigned.SurroundWith('<', '>'), paintEventArgs);
			}
			else
			{
				paintEventArgs.PaintText(data, paintEventArgs.Brush);
			}
		}
	}
}
