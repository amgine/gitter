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

using System;
using System.Drawing;

using gitter.Framework.Controls;
using gitter.Framework.Configuration;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>"Subject" column.</summary>
public sealed class SubjectColumn : CustomListBoxColumn
{
	public const int PointerTagHitOffset = 1;

	public const bool DefaultAlignToGraph = false;
	public const bool DefaultShowTags = true;
	public const bool DefaultShowLocalBranches = true;
	public const bool DefaultShowRemoteBranches = true;
	public const bool DefaultShowStash = true;

	private readonly bool _enableExtender;
	private bool _alignToGraph       = DefaultAlignToGraph;
	private bool _showLocalBranches  = DefaultShowLocalBranches;
	private bool _showRemoteBranches = DefaultShowRemoteBranches;
	private bool _showTags           = DefaultShowTags;
	private bool _showStash          = DefaultShowStash;

	private SubjectColumnExtender _extender;

	public event EventHandler AlignToGraphChanged;
	public event EventHandler ShowLocalBranchesChanged;
	public event EventHandler ShowRemoteBranchesChanged;
	public event EventHandler ShowTagsChanged;
	public event EventHandler ShowStashChanged;

	/// <summary>Create <see cref="SubjectColumn"/>.</summary>
	/// <param name="painter">Painter for cell data.</param>
	/// <param name="enableExtender">Enable column extender menu.</param>
	/// <exception cref="ArgumentNullException"><paramref name="painter"/> == <c>null</c>.</exception>
	public SubjectColumn(ISubItemPainter painter, bool enableExtender = true)
		: base((int)ColumnId.Subject, Resources.StrSubject, visible: true)
	{
		Verify.Argument.IsNotNull(painter);

		Painter  = painter;
		SizeMode = ColumnSizeMode.Fill;

		_enableExtender = enableExtender;
	}

	private ISubItemPainter Painter { get; }

	/// <inheritdoc/>
	public override string IdentificationString => "Subject";

	/// <summary>Align text and tags to graph column, if it is possible.</summary>
	public bool AlignToGraph
	{
		get => _alignToGraph;
		set
		{
			if(_alignToGraph != value)
			{
				_alignToGraph = value;
				if(!value && PreviousVisibleColumn is { Id: (int)ColumnId.Graph } prev)
				{
					prev.InvalidateContent();
				}
				InvalidateContent();
				AlignToGraphChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	/// <summary>Draw tags for all local branches, pointing to revision.</summary>
	public bool ShowLocalBranches
	{
		get => _showLocalBranches;
		set
		{
			if(_showLocalBranches != value)
			{
				_showLocalBranches = value;
				InvalidateContent();
				ShowLocalBranchesChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	/// <summary>Draw tags for all remote branches, pointing to revision.</summary>
	public bool ShowRemoteBranches
	{
		get => _showRemoteBranches;
		set
		{
			if(_showRemoteBranches != value)
			{
				_showRemoteBranches = value;
				InvalidateContent();
				ShowRemoteBranchesChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	/// <summary>Draw pointers for all tags, pointing to revision.</summary>
	public bool ShowTags
	{
		get => _showTags;
		set
		{
			if(_showTags != value)
			{
				_showTags = value;
				InvalidateContent();
				ShowTagsChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	/// <summary>Draw stash tag, if it is pointing to revision.</summary>
	public bool ShowStash
	{
		get => _showStash;
		set
		{
			if(_showStash != value)
			{
				_showStash = value;
				InvalidateContent();
				ShowStashChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	/// <summary>This column affects content of the column to the left.</summary>
	public override bool ExtendsToLeft => PreviousVisibleColumn is { Id: (int)ColumnId.Graph };

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		if(Painter.TryMeasure(measureEventArgs, out var size))
		{
			return size;
		}
		return Size.Empty;
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		Painter.TryPaint(paintEventArgs);
	}

	/// <inheritdoc/>
	protected override void OnListBoxAttached()
	{
		base.OnListBoxAttached();
		if(_enableExtender)
		{
			_extender = new SubjectColumnExtender(this);
			Extender = new Popup(_extender);
		}
	}

	/// <inheritdoc/>
	protected override void OnListBoxDetached()
	{
		if(_enableExtender)
		{
			Extender.Dispose();
			Extender = null;
			_extender.Dispose();
			_extender = null;
		}
		base.OnListBoxDetached();
	}

	/// <inheritdoc/>
	protected override void SaveMoreTo(Section section)
	{
		base.SaveMoreTo(section);
		section.SetValue("AlignToGraph", AlignToGraph);
		section.SetValue("ShowLocalBranches", ShowLocalBranches);
		section.SetValue("ShowRemoteBranches", ShowRemoteBranches);
		section.SetValue("ShowTags", ShowTags);
		section.SetValue("ShowStash", ShowStash);
	}

	/// <inheritdoc/>
	protected override void LoadMoreFrom(Section section)
	{
		base.LoadMoreFrom(section);
		AlignToGraph = section.GetValue("AlignToGraph", AlignToGraph);
		ShowLocalBranches = section.GetValue("ShowLocalBranches", ShowLocalBranches);
		ShowRemoteBranches = section.GetValue("ShowRemoteBranches", ShowRemoteBranches);
		ShowTags  = section.GetValue("ShowTags", ShowTags);
		ShowStash = section.GetValue("ShowStash", ShowStash);
	}
}
