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
	sealed class ParentsElement(RevisionHeaderContent owner) : BaseElement(owner)
	{
		private static readonly string SingleParentHeaderText    = Resources.StrParent.AddColon();
		private static readonly string MultipleParentsHeaderText = Resources.StrParents.AddColon();

		public override Element Element => Element.Parents;

		public override ContextMenuStrip CreateContextMenu(Revision revision)
		{
			Assert.IsNotNull(revision);

			var menu        = new ContextMenuStrip() { Renderer = GitterApplication.Style.ToolStripRenderer };
			var dpiBindings = new DpiBindings(menu);
			var factory     = new GuiItemFactory(dpiBindings);

			menu.Items.Add(factory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCopyToClipboard, CombineHashes(revision.Parents)));
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		public override bool IsAvailableFor(Revision revision)
			=> !revision.Parents.IsEmpty;

		private static Font GetFont(Dpi dpi)
			=> HashColumn.Font.GetValue(dpi);

		private static string CombineHashes(Many<Revision> revisions)
		{
			if(revisions.Count == 0) return "";
			if(revisions.Count == 1) return revisions.First().HashString;
			var first = true;
			int index = 0;
			var text = new char[(Sha1Hash.HexStringLength + 1) * revisions.Count - 1];
			foreach(var p in revisions)
			{
				if(!first) text[index++] = '\n';
				else first = false;
				p.HashString.CopyTo(0, text, index, Sha1Hash.HexStringLength);
				index += Sha1Hash.HexStringLength;
			}
			return new(text);
		}

		public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			return MeasureMultilineContent(graphics, dpi, GetFont(dpi), CombineHashes(revision.Parents), width);
		}

		public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			var parents = revision.Parents;
			var header  = parents.Count > 1 ? MultipleParentsHeaderText : SingleParentHeaderText;
			DefaultPaint(graphics, dpi, GetFont(dpi), header, CombineHashes(parents), rect);
		}
	}
}
