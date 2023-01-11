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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

partial class RevisionHeaderContent
{
	sealed class ReferencesElement : BaseElement
	{
		private struct ReferenceVisual
		{
			public ReferenceVisual(Reference reference, Rectangle rectangle)
			{
				Reference = reference;
				Rectangle = rectangle;
			}

			public Reference Reference { get; }

			public Rectangle Rectangle { get; }
		}

		private readonly List<ReferenceVisual> _drawnReferences = new();

		public ReferencesElement(RevisionHeaderContent owner)
			: base(owner)
		{
		}

		public override bool IsAvailableFor(Revision revision)
			=> revision.References.Count != 0;

		public override Element Element => Element.References;

		public override void MouseDown(Rectangle rect, MouseButtons button, int x, int y)
		{
			if(button == MouseButtons.Right)
			{
				foreach(var reference in _drawnReferences)
				{
					if(reference.Rectangle.X <= x && reference.Rectangle.Right > x)
					{
						ContextMenuStrip menu = reference.Reference switch
						{
							BranchBase branch => new BranchMenu(branch),
							Tag        tag    => new TagMenu(tag),
							Head       head   => new HeadMenu(head),
							_ => default,
						};
						if(menu is not null)
						{
							Utility.MarkDropDownForAutoDispose(menu);
							Owner.OnContextMenuRequested(menu, new Point(x + 1, y + 1));
						}
						return;
					}
				}
			}
		}

		public override Size Measure(Graphics graphics, Dpi dpi, Revision revision, int width)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			var conv = DpiConverter.FromDefaultTo(dpi);
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			lock(revision.References.SyncRoot)
			{
				if(revision.References.Count == 0) return Size.Empty;
				int offset = 0;
				foreach(var reference in revision.References)
				{
					var name = ((INamedObject)reference).Name;
					var size = GitterApplication.TextRenderer.MeasureText(graphics, name, font, int.MaxValue, ContentFormat);
					offset += size.Width + conv.ConvertX(3) + conv.ConvertX(6);
				}
				return new Size(conv.ConvertX(HeaderWidth) + offset - conv.ConvertX(3), conv.ConvertY(DefaultElementHeight));
			}
		}

		public override void Paint(Graphics graphics, Dpi dpi, Revision revision, Rectangle rect)
		{
			Assert.IsNotNull(graphics);
			Assert.IsNotNull(revision);

			const float Radius = 3;

			_drawnReferences.Clear();
			PaintHeader(graphics, dpi, Resources.StrRefs.AddColon(), rect);
			var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
			int offset = 0;
			lock(revision.References.SyncRoot)
			{
				if(revision.References.Count == 0) return;

				using var tagBrush    = SolidBrushCache.Get(ColorScheme.TagBackColor);
				using var localBrush  = SolidBrushCache.Get(ColorScheme.LocalBranchBackColor);
				using var remoteBrush = SolidBrushCache.Get(ColorScheme.RemoteBranchBackColor);

				var conv = DpiConverter.FromDefaultTo(dpi);
				var r2 = new Rectangle(rect.X + conv.ConvertX(HeaderWidth), rect.Y, rect.Width - conv.ConvertX(HeaderWidth), rect.Height);
				r2.Y += GetYOffset(conv.To, font);
				foreach(var reference in revision.References)
				{
					var name = ((INamedObject)reference).Name;
					var size = GitterApplication.TextRenderer.MeasureText(
						graphics, name, font, int.MaxValue, ContentFormat);
					var r = new Rectangle(r2.X + offset, r2.Y + 1, size.Width + conv.ConvertX(6), conv.ConvertY(DefaultElementHeight));
					var brush = reference.Type switch
					{
						ReferenceType.LocalBranch  => localBrush,
						ReferenceType.RemoteBranch => remoteBrush,
						ReferenceType.Tag          => tagBrush,
						_ => Brushes.WhiteSmoke,
					};
					graphics.FillRoundedRectangle(brush, Pens.Black, r, conv.ConvertX(Radius));
					var textRect = new Rectangle(r2.X + offset + conv.ConvertX(3), r2.Y, size.Width + conv.ConvertX(6) - 1, size.Height);
					GitterApplication.TextRenderer.DrawText(
						graphics, name, font, SystemBrushes.WindowText, textRect, ContentFormat);
					_drawnReferences.Add(new ReferenceVisual(reference, r));
					offset += size.Width + conv.ConvertX(3) + conv.ConvertX(6);
				}
			}
		}
	}
}
