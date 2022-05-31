#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Drawing;

using gitter.Framework.Controls;
using gitter.Framework.Configuration;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>"Graph" column.</summary>
public sealed class GraphColumn : CustomListBoxColumn
{
	public const bool DefaultShowColors     = false;
	public const bool DefaultFillBackground = false;

	#region Data

	private bool _showColors;
	private bool _fillBackground;
	private GraphColumnExtender _extender;

	#endregion

	#region Events

	public event EventHandler ShowColorsChanged;

	private void OnShowColorsChanged(EventArgs e)
		=> ShowColorsChanged?.Invoke(this, e);

	public event EventHandler FillBackgroundChanged;

	private void OnFillBackgroundChanged(EventArgs e)
		=> FillBackgroundChanged?.Invoke(this, e);

	#endregion

	public GraphColumn(IGraphStyle graphStyle)
		: base((int)ColumnId.Graph, Resources.StrGraph, visible: true)
	{
		Verify.Argument.IsNotNull(graphStyle);

		GraphStyle = graphStyle;

		Width    = 16 + 5;
		SizeMode = ColumnSizeMode.Fixed;

		_showColors     = DefaultShowColors;
		_fillBackground = DefaultFillBackground;
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached()
	{
		base.OnListBoxAttached();
		GraphStyle.Changed += OnGraphStyleChanged;
		_extender = new GraphColumnExtender(this);
		Extender = new Popup(_extender);
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached()
	{
		GraphStyle.Changed -= OnGraphStyleChanged;
		if(Extender is not null)
		{
			Extender.Dispose();
			Extender = null;
		}
		if(_extender is not null)
		{
			_extender.Dispose();
			_extender = null;
		}
		base.OnListBoxDetached();
	}

	private IGraphStyle GraphStyle { get; }

	public bool ShowColors
	{
		get => _showColors;
		set
		{
			if(_showColors != value)
			{
				_showColors = value;
				InvalidateContent();
				OnShowColorsChanged(EventArgs.Empty);
			}
		}
	}

	public bool FillBackground
	{
		get => _fillBackground;
		set
		{
			if(_fillBackground != value)
			{
				_fillBackground = value;
				InvalidateContent();
				OnFillBackgroundChanged(EventArgs.Empty);
			}
		}
	}

	private void OnGraphStyleChanged(object sender, EventArgs e)
	{
		InvalidateContent();
	}

	private static bool TryGetGraph(CustomListBoxItem item, out GraphCell[] graph, out RevisionGraphItemType type)
	{
		switch(item)
		{
			case RevisionListItem revItem:
				graph = revItem.Graph;
				type  = revItem.DataContext.IsCurrent
					? RevisionGraphItemType.Current
					: RevisionGraphItemType.Generic;
				return graph is not null;
			case FakeRevisionListItem fakeRevItem:
				graph = fakeRevItem.Graph;
				type  = fakeRevItem.Type == FakeRevisionItemType.StagedChanges
					? RevisionGraphItemType.Uncommitted
					: RevisionGraphItemType.Unstaged;
				return graph is not null;
			default:
				graph = default;
				type  = default;
				return false;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		if(!TryGetGraph(paintEventArgs.Item, out var graph, out var itemType)) return;

		var selected       = (paintEventArgs.State & ItemState.Selected) == ItemState.Selected;
		var useColors      = ShowColors && (!selected || !paintEventArgs.IsHostControlFocused);
		var fillBackground = FillBackground;

		var bounds = paintEventArgs.Bounds;
		var h      = paintEventArgs.ListBox.CurrentItemHeight;

		if(fillBackground)
		{
			if(NextVisibleColumn is SubjectColumn { AlignToGraph: true } subject)
			{
				int i = graph.Length - 1;
				while(i > 0 && graph[i].Elements == GraphElement.Space) --i;
				bounds.Width = h * (i + 1);
			}
			GraphStyle.DrawBackground(
				paintEventArgs.Graphics, paintEventArgs.Dpi,
				graph, bounds, paintEventArgs.ClipRectangle, h, useColors);
		}

		GraphStyle.DrawGraph(
			paintEventArgs.Graphics, paintEventArgs.Dpi,
			graph, bounds, paintEventArgs.ClipRectangle, h, itemType, useColors);
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(!TryGetGraph(measureEventArgs.Item, out var graph, out _)) return Size.Empty;

		var h = measureEventArgs.ListBox.CurrentItemHeight;
		return new Size(graph.Length * h, h);
	}

	/// <inheritdoc/>
	protected override void SaveMoreTo(Section section)
	{
		base.SaveMoreTo(section);
		section.SetValue("ShowColors",     ShowColors);
		section.SetValue("FillBackground", FillBackground);
	}

	/// <inheritdoc/>
	protected override void LoadMoreFrom(Section section)
	{
		base.LoadMoreFrom(section);
		ShowColors     = section.GetValue("ShowColors", ShowColors);
		FillBackground = section.GetValue("FillBackground", FillBackground);
	}

	/// <inheritdoc/>
	public override string IdentificationString => @"Graph";
}
