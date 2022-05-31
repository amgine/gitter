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

namespace gitter.Git.Gui.Controls;

using System.Collections.Generic;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

/// <summary>Paints <see cref="Revision.Subject"/> for the <see cref="SubjectColumn"/>.</summary>
abstract class RevisionSubjectPainterBase<T> : ISubItemPainter
	where T : CustomListBoxItem
{
	private static readonly IDpiBoundValue<int> TagSpacing = DpiBoundValue.ScaleX(2);

	public RevisionSubjectPainterBase(IGraphStyle graphStyle)
	{
		Verify.Argument.IsNotNull(graphStyle);

		GraphStyle = graphStyle;
	}

	private IGraphStyle GraphStyle { get; }

	protected abstract bool TryGetData(T item, out Revision revision, out GraphCell[] graph, out List<PointerBounds> drawnPointers);

	public virtual bool TryMeasure(SubItemMeasureEventArgs measureEventArgs, out Size size)
	{
		Verify.Argument.IsNotNull(measureEventArgs);

		if( measureEventArgs.Item   is not T item ||
			measureEventArgs.Column is not SubjectColumn)
		{
			size = Size.Empty;
			return false;
		}

		if(!TryGetData(item, out var revision, out _, out _))
		{
			size = Size.Empty;
			return false;
		}

		size = measureEventArgs.MeasureText(revision.Subject);
		return true;
	}

	public bool TryPaint(SubItemPaintEventArgs paintEventArgs)
	{
		Verify.Argument.IsNotNull(paintEventArgs);

		if(paintEventArgs.Item   is not T item)               return false;
		if(paintEventArgs.Column is not SubjectColumn column) return false;

		if(!TryGetData(item, out var revision, out var graph, out var drawnPointers))
		{
			return false;
		}

		var hoveredPointer = paintEventArgs.HoveredPart - SubjectColumn.PointerTagHitOffset;

		#region get painting options

		bool showLocalBranches;
		bool showRemoteBranches;
		bool showTags;
		bool showStash;
		bool alignToGraph;
		if(drawnPointers is not null)
		{
			drawnPointers.Clear();
			alignToGraph = column.AlignToGraph;
			showLocalBranches = column.ShowLocalBranches;
			showRemoteBranches = column.ShowRemoteBranches;
			showTags = column.ShowTags;
			showStash = column.ShowStash;
		}
		else
		{
			alignToGraph = column.AlignToGraph;
			showLocalBranches = false;
			showRemoteBranches = false;
			showTags = false;
			showStash = false;
		}

		#endregion

		#region align to graph column

		var rect = paintEventArgs.Bounds;
		var graphColumn = paintEventArgs.Column.PreviousVisibleColumn;
		int graphColumnX = 0;
		if(graphColumn is not null)
		{
			if(graphColumn.Id != (int)ColumnId.Graph)
			{
				graphColumn = null;
			}
			else
			{
				graphColumnX = rect.X - graphColumn.CurrentWidth;
			}
		}
		if(graphColumn is not null && alignToGraph)
		{
			var itemHeight = paintEventArgs.ListBox.CurrentItemHeight;
			int availWidth = graphColumn.CurrentWidth - itemHeight * graph.Length;
			for(int i = graph.Length - 1; i >= 0; --i)
			{
				if(!graph[i].IsEmpty) break;
				availWidth += itemHeight;
			}
			if(availWidth != 0)
			{
				rect.X -= availWidth;
				rect.Width += availWidth;
			}
		}

		#endregion

		#region prepare to draw references

		int drawnRefs = 0;
		int xoffset = 0;
		var font = paintEventArgs.Column.ContentFont;
		paintEventArgs.PrepareContentRectangle(ref rect);

		#endregion

		#region paint tag & branch refs

		bool refsPresent;
		lock(revision.References.SyncRoot)
		{
			refsPresent = revision.References.Count != 0;
			if((showLocalBranches || showRemoteBranches || showTags) && refsPresent)
			{
				foreach(var reference in revision.References)
				{
					var bounds = default(Rectangle);
					switch(reference.Type)
					{
						case ReferenceType.LocalBranch when showLocalBranches:
							bounds = GraphStyle.DrawBranch(
								paintEventArgs.Graphics, paintEventArgs.Dpi,
								font,
								GitterApplication.TextRenderer.LeftAlign,
								rect.Left + xoffset,
								rect.Top,
								rect.Right,
								rect.Height,
								drawnRefs == hoveredPointer,
								(Branch)reference);
							break;
						case ReferenceType.RemoteBranch when showRemoteBranches:
							bounds = GraphStyle.DrawBranch(
								paintEventArgs.Graphics, paintEventArgs.Dpi,
								font,
								GitterApplication.TextRenderer.LeftAlign,
								rect.Left + xoffset,
								rect.Top,
								rect.Right,
								rect.Height,
								drawnRefs == hoveredPointer,
								(RemoteBranch)reference);
							break;
						case ReferenceType.Tag when showTags:
							bounds = GraphStyle.DrawTag(
								paintEventArgs.Graphics, paintEventArgs.Dpi,
								font,
								GitterApplication.TextRenderer.LeftAlign,
								rect.Left + xoffset,
								rect.Top,
								rect.Right,
								rect.Height,
								drawnRefs == hoveredPointer,
								(Tag)reference);
							break;
					}
					if(bounds.Width > 0)
					{
						bounds.Y -= paintEventArgs.Bounds.Y;
						drawnPointers.Add(new PointerBounds(reference, bounds));
						xoffset += bounds.Width + TagSpacing.GetValue(paintEventArgs.Dpi);
						++drawnRefs;
					}
					if(xoffset >= rect.Width) break;
				}
			}
		}

		#endregion

		#region paint stash ref

		var stash = revision.Repository.Stash.MostRecentState;
		var stashPresent = stash is not null && revision == stash.Revision;
		if(showStash && stashPresent)
		{
			if(xoffset < rect.Width)
			{
				var bounds = GraphStyle.DrawStash(
					paintEventArgs.Graphics, paintEventArgs.Dpi,
					font,
					GitterApplication.TextRenderer.LeftAlign,
					rect.Left + xoffset,
					rect.Top,
					rect.Right,
					rect.Height,
					drawnRefs == hoveredPointer,
					stash);
				if(bounds.Width > 0)
				{
					bounds.Y -= paintEventArgs.Bounds.Y;
					drawnPointers.Add(new PointerBounds(stash, bounds));
					xoffset += bounds.Width + TagSpacing.GetValue(paintEventArgs.Dpi);
					++drawnRefs;
				}
			}
		}

		#endregion

		#region paint reference presence indication

		if(drawnRefs != 0)
		{
			if(graph is { Length: > 0 })
			{
				if(graphColumn is not null)
				{
					GraphStyle.DrawReferenceConnector(paintEventArgs.Graphics, paintEventArgs.Dpi,
						graph, graphColumnX, paintEventArgs.ListBox.CurrentItemHeight, rect.X, rect.Y, rect.Height);
				}
			}
			xoffset += TagSpacing.GetValue(paintEventArgs.Dpi);
		}
		else
		{
			if(refsPresent || stashPresent)
			{
				if(graph is { Length: > 0 } && graphColumn is not null)
				{
					GraphStyle.DrawReferencePresenceIndicator(paintEventArgs.Graphics, paintEventArgs.Dpi,
						graph, graphColumnX, paintEventArgs.ListBox.CurrentItemHeight, rect.Y, rect.Height);
				}
			}
		}

		#endregion

		#region paint subject text

		rect.X += xoffset;
		rect.Width -= xoffset;
		if(rect.Width > 1)
		{
			paintEventArgs.PrepareTextRectangle(font, ref rect);
			GitterApplication.TextRenderer.DrawText(
				paintEventArgs.Graphics, revision.Subject, font, paintEventArgs.Brush, rect);
		}

		#endregion

		return true;
	}
}
