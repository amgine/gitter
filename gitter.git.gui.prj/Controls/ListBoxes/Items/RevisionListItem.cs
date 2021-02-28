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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	/// <summary><see cref="CustomListBoxItem"/>, representing <see cref="Revision"/>.</summary>
	public class RevisionListItem : RevisionPointerListItemBase<Revision>, IRevisionGraphListItem
	{
		#region Constants

		private const int PointerTagHitOffset = 1;

		#endregion

		#region Data

		private readonly List<PointerBounds> _drawnPointers = new();

		#endregion

		#region .ctor

		/// <summary>Create <see cref="RevisionListItem"/>.</summary>
		/// <param name="revision">Associated revision.</param>
		public RevisionListItem(Revision revision)
			: base(revision)
		{
		}

		#endregion

		#region Properties

		public GraphAtom[] Graph { get; set; }

		#endregion

		#region Methods

		private static bool IsAlignToGraphEnabled(CustomListBoxColumn column)
			=> column is SubjectColumn subjectColumn && subjectColumn.AlignToGraph;

		private void DrawBranchDragImage(Branch branch, Graphics graphics, Size size)
		{
			graphics.TextRenderingHint = GraphicsUtility.TextRenderingHint;
			graphics.TextContrast      = GraphicsUtility.TextContrast;
			GlobalBehavior.GraphStyle.DrawBranch(
				graphics,
				ListBox.Font,
				GitterApplication.TextRenderer.LeftAlign,
				0, 0, size.Width, size.Height,
				true, branch);
		}

		#endregion

		#region Overrides

		protected override void OnListBoxAttached()
		{
			DataContext.References.Changed += OnReferenceListChanged;
			base.OnListBoxAttached();
		}

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

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = default(ContextMenuStrip);
			if(requestEventArgs.Column != null)
			{
				switch((ColumnId)requestEventArgs.SubItemId)
				{
					case ColumnId.Graph:
						var ncid = ListBox.GetNextVisibleColumnIndex(requestEventArgs.ColumnIndex);
						if(ncid >= 0 && ncid < ListBox.Columns.Count)
						{
							var column = ListBox.Columns[ncid];
							if(IsAlignToGraphEnabled(column))
							{
								return GetContextMenu(new ItemContextMenuRequestEventArgs(
									requestEventArgs.Item, column, ncid,
									requestEventArgs.X, requestEventArgs.Y));
							}
						}
						break;
					case ColumnId.Name:
					case ColumnId.Subject:
						menu = PointerBounds.GetContextMenu(_drawnPointers, requestEventArgs.X, requestEventArgs.Y);
						break;
				}
			}
			menu ??= new RevisionMenu(DataContext);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}

		public override void OnMouseDown(MouseButtons button, int x, int y)
		{
			if(button == MouseButtons.Left)
			{
				if(_drawnPointers != null)
				{
					for(int i = 0; i < _drawnPointers.Count; ++i)
					{
						var rc = _drawnPointers[i].Bounds;
						if(rc.X <= x && rc.Right > x)
						{
							if(_drawnPointers[i].RevisionPointer is Branch branch && !branch.IsRemote)
							{
								int dx = _drawnPointers[i].Bounds.X - x - 1;
								var w = GlobalBehavior.GraphStyle.MeasureBranch(
									GraphicsUtility.MeasurementGraphics, ListBox.Font, GitterApplication.TextRenderer.LeftAlign, branch);
								var h = ListBox.ItemHeight;
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
								if(_drawnPointers[i].RevisionPointer is Tag tag && tag.TagType == TagType.Annotated)
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

		protected override int OnHitTest(int x, int y)
		{
			if(_drawnPointers != null)
			{
				for(int i = 0; i < _drawnPointers.Count; ++i)
				{
					var rc = _drawnPointers[i].Bounds;
					if(rc.X <= x && rc.Right > x)
					{
						return PointerTagHitOffset + i;
					}
				}
			}
			return base.OnHitTest(x, y);
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Subject:
					return SubjectColumn.OnMeasureSubItem(measureEventArgs, DataContext, Graph);
				case ColumnId.Date:
				case ColumnId.Committer:
					return CommitterColumn.OnMeasureSubItem(measureEventArgs, DataContext.Committer);
				case ColumnId.CommitterEmail:
					return CommitterEmailColumn.OnMeasureSubItem(measureEventArgs, DataContext.Committer.Email);
				case ColumnId.User:
				case ColumnId.Author:
					return AuthorColumn.OnMeasureSubItem(measureEventArgs, DataContext.Author);
				case ColumnId.AuthorEmail:
					return AuthorEmailColumn.OnMeasureSubItem(measureEventArgs, DataContext.Author.Email);
				default:
					return base.OnMeasureSubItem(measureEventArgs);
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
				case ColumnId.Subject:
					SubjectColumn.OnPaintSubItem(paintEventArgs, DataContext, Graph, _drawnPointers, paintEventArgs.HoveredPart - PointerTagHitOffset);
					break;
				case ColumnId.Committer:
					CommitterColumn.OnPaintSubItem(paintEventArgs, DataContext.Committer);
					break;
				case ColumnId.CommitterEmail:
					CommitterEmailColumn.OnPaintSubItem(paintEventArgs, DataContext.Committer.Email);
					break;
				case ColumnId.User:
				case ColumnId.Author:
					AuthorColumn.OnPaintSubItem(paintEventArgs, DataContext.Author);
					break;
				case ColumnId.AuthorEmail:
					AuthorEmailColumn.OnPaintSubItem(paintEventArgs, DataContext.Author.Email);
					break;
				default:
					base.OnPaintSubItem(paintEventArgs);
					break;
			}
		}

		#endregion
	}
}
