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
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

partial class RevisionHeaderContent
{
	sealed class TreeHashElement : BaseElement
	{
		private static readonly string HeaderText = Resources.StrTreeHash.AddColon();

		public TreeHashElement(RevisionHeaderContent owner)
			: base(owner)
		{
		}

		public override Element Element => Element.TreeHash;

		public override ContextMenuStrip CreateContextMenu(Revision revision)
		{
			Assert.IsNotNull(revision);

			var menu        = new ContextMenuStrip();
			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);

			menu.Items.Add(factory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, revision.TreeHashString));

			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
			=> Measure(graphics, dpi, TreeHashColumn.Font.GetValue(dpi), revision.TreeHashString, width);

		public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
			=> DefaultPaint(graphics, dpi, TreeHashColumn.Font.GetValue(dpi), HeaderText, revision.TreeHashString, rect);
	}
}
