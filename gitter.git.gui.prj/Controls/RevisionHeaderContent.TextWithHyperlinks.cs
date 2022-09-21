﻿#region Copyright Notice
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
using gitter.Framework.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

partial class RevisionHeaderContent
{
	abstract class TextWithHyperlinksElementBase : BaseElement
	{
		private readonly string _headerText;
		private TextWithHyperlinks _text;

		protected TextWithHyperlinksElementBase(RevisionHeaderContent owner, string headerText)
			: base(owner)
		{
			_headerText = headerText;
		}

		protected abstract string GetText(Revision revision);

		public override ContextMenuStrip CreateContextMenu(Revision revision, Rectangle bounds, int x, int y)
		{
			if(_text is not null)
			{
				var link = _text.HitTest(GetContentRectangle(bounds), new Point(x, y));
				if(link is not null) return new HyperlinkContextMenu(link);
			}
			var menu        = new ContextMenuStrip();
			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);
			menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard,
				GetText(revision), false));
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
			=> MeasureMultilineContent(graphics, dpi, GetText(revision), width);

		public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			var text = GetText(revision);
			if(_text is null || _text.Text != text)
			{
				_text = new TextWithHyperlinks(text, GetHyperlinkExtractor(revision));
				_text.InvalidateRequired += OnTextInvalidateRequired;
			}

			DefaultPaint(graphics, dpi, _headerText, _text, rect);
		}

		private void OnTextInvalidateRequired(object sender, EventArgs e)
		{
			OnInvalidateRequired();
			if(_text.HoveredHyperlink is null)
			{
				ChangeCursor(Cursors.Default);
			}
			else
			{
				ChangeCursor(Cursors.Hand);
			}
		}

		public override void MouseMove(Rectangle rect, Point point)
			=> _text?.OnMouseMove(GetContentRectangle(rect), point);

		public override void MouseLeave()
			=> _text?.OnMouseLeave();

		public override void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
		{
			if(_text is not null)
			{
				switch(button)
				{
					case MouseButtons.Left:
						_text.OnMouseDown(GetContentRectangle(rect), new Point(x, y));
						break;
				}
			}
		}
	}
}
