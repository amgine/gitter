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
using System.Text;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

partial class RevisionHeaderContent
{
	sealed class ParentsElement : BaseElement
	{
		private static readonly string SingleParentHeaderText    = Resources.StrParent.AddColon();
		private static readonly string MultipleParentsHeaderText = Resources.StrParents.AddColon();

		public ParentsElement(RevisionHeaderContent owner)
			: base(owner)
		{
		}

		public override Element Element => Element.Parents;

		public override ContextMenuStrip CreateContextMenu(Revision revision)
		{
			Assert.IsNotNull(revision);

			var menu        = new ContextMenuStrip();
			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);

			var sb = new StringBuilder(41 * revision.Parents.Count);
			foreach(var p in revision.Parents)
			{
				sb.Append(p.Hash);
				sb.Append('\n');
			}
			sb.Remove(sb.Length - 1, 1);
			menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, sb.ToString()));

			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		private static Font GetFont(Dpi dpi)
			=> HashColumn.Font.GetValue(dpi);

		public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			switch(revision.Parents.Count)
			{
				case 0:
					return Size.Empty;
				case 1:
					return Measure(graphics, dpi, GetFont(dpi), revision.Parents[0].HashString, width);
				default:
					var sb = new StringBuilder(41 * revision.Parents.Count);
					bool first = true;
					for(int i = 0; i < revision.Parents.Count; ++i)
					{
						var p = revision.Parents[i];
						if(!first) sb.Append('\n');
						else first = false;
						sb.Append(p.Hash);
					}
					return MeasureMultilineContent(graphics, dpi, GetFont(dpi), sb.ToString(), width);
			}
		}

		public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			if(revision.Parents.Count == 1)
			{
				DefaultPaint(graphics, dpi, GetFont(dpi), SingleParentHeaderText, revision.Parents[0].HashString, rect);
			}
			else
			{
				var sb = new StringBuilder(41 * revision.Parents.Count);
				foreach(var p in revision.Parents)
				{
					sb.Append(p.Hash);
					sb.Append('\n');
				}
				DefaultPaint(graphics, dpi, GetFont(dpi), MultipleParentsHeaderText, sb.ToString(), rect);
			}
		}
	}
}
