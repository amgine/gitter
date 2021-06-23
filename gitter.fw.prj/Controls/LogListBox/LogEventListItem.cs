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
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	sealed class LogEventListItem : CustomListBoxItem<LogEvent>
	{
		public LogEventListItem(LogEvent logEvent)
			: base(logEvent)
		{
		}

		/// <inheritdoc/>
		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			=> (LogListBoxColumnId)measureEventArgs.SubItemId switch
			{
				LogListBoxColumnId.Type      => new Size(16, 16),
				LogListBoxColumnId.Timestamp => measureEventArgs.MeasureText(DataContext.Timestamp.FormatISO8601()),
				LogListBoxColumnId.Source    => measureEventArgs.MeasureText(DataContext.Source),
				LogListBoxColumnId.Message   => measureEventArgs.MeasureText(DataContext.Message),
				LogListBoxColumnId.Exception => new Size(16, 16),
				_ => Size.Empty,
			};

		/// <inheritdoc/>
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			switch((LogListBoxColumnId)paintEventArgs.SubItemId)
			{
				case LogListBoxColumnId.Type:
					paintEventArgs.PaintImage(DataContext.Type.Image);
					break;
				case LogListBoxColumnId.Timestamp:
					paintEventArgs.PaintText(DataContext.Timestamp.FormatISO8601());
					break;
				case LogListBoxColumnId.Source:
					paintEventArgs.PaintText(DataContext.Source);
					break;
				case LogListBoxColumnId.Message:
					paintEventArgs.PaintText(DataContext.Message);
					break;
				case LogListBoxColumnId.Exception:
					paintEventArgs.PaintImage(null);
					break;
			}
		}

		/// <inheritdoc/>
		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			Assert.IsNotNull(requestEventArgs);

			var menu = new ContextMenuStrip();
			menu.Items.Add(new ToolStripMenuItem("Copy to Clipboard", null, (_, _) => ClipboardEx.SetTextSafe(DataContext.Message)));
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
