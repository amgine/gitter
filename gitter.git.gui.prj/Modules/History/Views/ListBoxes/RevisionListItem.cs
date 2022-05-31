#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary><see cref="CustomListBoxItem"/>, representing <see cref="Revision"/>.</summary>
public class RevisionListItem : RevisionPointerListItemBase<Revision>, IRevisionGraphListItem
{
	private readonly List<PointerBounds> _drawnPointers = new();

	/// <summary>Create <see cref="RevisionListItem"/>.</summary>
	/// <param name="revision">Associated revision.</param>
	public RevisionListItem(Revision revision)
		: base(revision)
	{
	}

	public GraphCell[] Graph { get; set; }

	internal List<PointerBounds> DrawnPointers => _drawnPointers;

	private void DrawBranchDragImage(Branch branch, Graphics graphics, Size size)
	{
		var dpi  = Dpi.FromControl(ListBox);
		var font = GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi);
		graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
		graphics.TextContrast      = GraphicsUtility.TextContrast;
		GlobalBehavior.GraphStyle.DrawBranch(
			graphics, dpi,
			font,
			GitterApplication.TextRenderer.LeftAlign,
			0, 0, size.Width, size.Height,
			true, branch);
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached()
	{
		DataContext.References.Changed += OnReferenceListChanged;
		base.OnListBoxAttached();
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached()
	{
		DataContext.References.Changed -= OnReferenceListChanged;
		_drawnPointers?.Clear();
		base.OnListBoxDetached();
	}

	private void OnReferenceListChanged(object sender, EventArgs e)
	{
		InvalidateSafe();
	}

	/// <inheritdoc/>
	public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
	{
		Assert.IsNotNull(requestEventArgs);

		var menu = default(ContextMenuStrip);
		if(requestEventArgs.Column is not null)
		{
			switch((ColumnId)requestEventArgs.SubItemId)
			{
				case ColumnId.Graph:
					var ncid = ListBox.GetNextVisibleColumnIndex(requestEventArgs.ColumnIndex);
					if(ncid >= 0 && ncid < ListBox.Columns.Count)
					{
						if(ListBox.Columns[ncid] is SubjectColumn { AlignToGraph: true } subjectColumn)
						{
							return GetContextMenu(new ItemContextMenuRequestEventArgs(
								requestEventArgs.Item, subjectColumn, requestEventArgs.ItemBounds, ncid,
								requestEventArgs.X, requestEventArgs.Y));
						}
					}
					break;
				case ColumnId.Name:
				case ColumnId.Subject:
					var x = requestEventArgs.X - requestEventArgs.ItemBounds.X;
					var y = requestEventArgs.Y - requestEventArgs.ItemBounds.Y;
					menu = PointerBounds.GetContextMenu(_drawnPointers, x, y);
					break;
			}
		}
		menu ??= new RevisionMenu(DataContext);
		Utility.MarkDropDownForAutoDispose(menu);
		return menu;
	}

	/// <inheritdoc/>
	public override void OnMouseDown(MouseButtons button, int x, int y)
	{
		if(button == MouseButtons.Left)
		{
			if(_drawnPointers is not null)
			{
				for(int i = 0; i < _drawnPointers.Count; ++i)
				{
					if(GlobalBehavior.GraphStyle.HitTestReference(_drawnPointers[i].Bounds, x, y))
					{
						if(_drawnPointers[i].RevisionPointer is Branch { IsRemote: false } branch)
						{
							var dpi = Dpi.FromControl(ListBox);
							int dx = _drawnPointers[i].Bounds.X - x - 1;
							var w = GlobalBehavior.GraphStyle.MeasureBranch(
								GraphicsUtility.MeasurementGraphics,
								dpi,
								GitterApplication.FontManager.UIFont.ScalableFont.GetValue(dpi),
								GitterApplication.TextRenderer.LeftAlign,
								branch);
							var h = ListBox.CurrentItemHeight;
							var size = new Size(w, h);
							using(var dragImage = new DragImage(size, -dx, y,
								eargs => DrawBranchDragImage(branch, eargs.Graphics, size)))
							{
								dragImage.ShowDragVisual(ListBox);
								ListBox.DoDragDrop(branch, DragDropEffects.None | DragDropEffects.Scroll | DragDropEffects.Move);
							}
							return;
						}
						else
						{
							if(_drawnPointers[i].RevisionPointer is Tag { TagType: TagType.Annotated } tag)
							{
								//var message = tag.Message;
								//if(!string.IsNullOrEmpty(message))
								//{
								//    var tt = new ToolTip()
								//    {
								//        AutomaticDelay = 0,
								//        AutoPopDelay = 0,
								//        IsBalloon = false,
								//        UseAnimation = false,
								//        UseFading = false,
								//        InitialDelay = 0,
								//        StripAmpersands = false,
								//    };
								//    tt.Show(message, ListBox);
								//}
							}
						}
					}
				}
			}
		}
		base.OnMouseDown(button, x, y);
	}

	/// <inheritdoc/>
	protected override int OnHitTest(int x, int y)
	{
		if(_drawnPointers is { Count: not 0 })
		{
			for(int i = 0; i < _drawnPointers.Count; ++i)
			{
				if(GlobalBehavior.GraphStyle.HitTestReference(_drawnPointers[i].Bounds, x, y))
				{
					return SubjectColumn.PointerTagHitOffset + i;
				}
			}
		}
		return base.OnHitTest(x, y);
	}
}
