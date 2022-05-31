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

namespace gitter.Git.Gui;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.Git.Gui.Properties.Resources;

partial class RevisionHeaderContent
{
	abstract class TimestampElement : BaseElement
	{
		protected TimestampElement(RevisionHeaderContent owner)
			: base(owner)
		{
			DateFormat = DateFormat.ISO8601;
		}

		public override ContextMenuStrip CreateContextMenu(Revision revision)
		{
			Assert.IsNotNull(revision);

			var menu        = new ContextMenuStrip();
			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);

			menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
				Utility.FormatDate(revision.CommitDate, DateFormat)));

			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		public DateFormat DateFormat { get; set; }

		protected abstract DateTimeOffset GetTimestamp(Revision revision);

		protected virtual string HeaderText { get; } = Resources.StrDate.AddColon();

		public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			var timestamp = GetTimestamp(revision);
			var font      = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			var text      = Utility.FormatDate(timestamp, DateFormat);
			return Measure(graphics, dpi, font, text, width);
		}

		public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			var timestamp = GetTimestamp(revision);
			var font      = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			var text      = Utility.FormatDate(timestamp, DateFormat);

			DefaultPaint(graphics, dpi, font, HeaderText, text, rect);
		}
	}
}
