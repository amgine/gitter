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
	abstract class UserElement : BaseElement
	{
		protected UserElement(RevisionHeaderContent owner)
			: base(owner)
		{
		}

		protected abstract User GetUser(Revision revision);

		public override ContextMenuStrip CreateContextMenu(Revision revision)
		{
			Assert.IsNotNull(revision);

			var menu        = new ContextMenuStrip();
			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);
			var user        = GetUser(revision);

			menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, GetText(user)));
			menu.Items.Add(factory.GetSendEmailItem<ToolStripMenuItem>(user.Email));

			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		protected abstract string HeaderText { get; }

		private static string GetText(User user)
			=> string.Format("{0} <{1}>", user.Name, user.Email);

		public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			var user = GetUser(revision);
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			return Measure(graphics, dpi, font, GetText(user), width);
		}

		public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			var user = GetUser(revision);
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			DefaultPaint(graphics, dpi, font, HeaderText, GetText(user), rect);
		}
	}
}
