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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
class SubjectColumnExtender : ExtenderBase
{
	#region Data

	private readonly DpiBindings _dpiBindings;
	private bool _disableEvents;

	private GroupSeparator _grpVisibleReferences;
	private ICheckBoxWidget _chkAlignToGraph;
	private ICheckBoxWidget _chkLocalBranches;
	private ICheckBoxWidget _chkRemoteBranches;
	private ICheckBoxWidget _chkTags;
	private ICheckBoxWidget _chkStash;

	#endregion

	#region .ctor

	public SubjectColumnExtender(SubjectColumn column)
		: base(column)
	{
		SuspendLayout();
		Name = nameof(SubjectColumnExtender);
		Size = new(214, 143);
		ResumeLayout();

		_dpiBindings = new DpiBindings(this);

		CreateControls();
		SubscribeToColumnEvents();
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			UnsubscribeFromColumnEvents();
		}
		base.Dispose(disposing);
	}

	#endregion

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(214, 143));

	/// <inheritdoc/>
	protected override bool ScaleChildren => Focused;

	#region Methods

	[MemberNotNull(nameof(_grpVisibleReferences))]
	[MemberNotNull(nameof(_chkAlignToGraph))]
	[MemberNotNull(nameof(_chkLocalBranches))]
	[MemberNotNull(nameof(_chkRemoteBranches))]
	[MemberNotNull(nameof(_chkTags))]
	[MemberNotNull(nameof(_chkStash))]
	private void CreateControls()
	{
		_dpiBindings.UnbindAll();

		_chkAlignToGraph?.Dispose();
		_chkAlignToGraph = Style.CheckBoxFactory.Create();
		_chkAlignToGraph.Text = Resources.StrAlignToGraph;
		_chkAlignToGraph.IsChecked = Column.AlignToGraph;
		_chkAlignToGraph.IsCheckedChanged += OnAlignToGraphCheckedChanged;
		_dpiBindings.BindImage(_chkAlignToGraph, Icons.AlignToGraph);

		_grpVisibleReferences ??= new GroupSeparator()
		{
			Text = Resources.StrVisibleReferences,
		};

		_chkLocalBranches?.Dispose();
		_chkLocalBranches = Style.CheckBoxFactory.Create();
		_chkLocalBranches.Text  = Resources.StrLocalBranches;
		_chkLocalBranches.IsChecked = Column.ShowLocalBranches;
		_chkLocalBranches.IsCheckedChanged += OnLocalBranchesCheckedChanged;
		_dpiBindings.BindImage(_chkLocalBranches, Icons.Branch);

		_chkRemoteBranches?.Dispose();
		_chkRemoteBranches = Style.CheckBoxFactory.Create();
		_chkRemoteBranches.Text = Resources.StrRemoteBranches;
		_chkRemoteBranches.IsChecked = Column.ShowRemoteBranches;
		_chkRemoteBranches.IsCheckedChanged += OnRemoteBranchesCheckedChanged;
		_dpiBindings.BindImage(_chkRemoteBranches, Icons.RemoteBranch);

		_chkTags?.Dispose();
		_chkTags = Style.CheckBoxFactory.Create();
		_chkTags.Text = Resources.StrTags;
		_chkTags.IsChecked = Column.ShowTags;
		_chkTags.IsCheckedChanged += OnTagsCheckedChanged;
		_dpiBindings.BindImage(_chkTags, Icons.Tag);

		_chkStash?.Dispose();
		_chkStash = Style.CheckBoxFactory.Create();
		_chkStash.Text = Resources.StrStash;
		_chkStash.IsChecked = Column.ShowStash;
		_chkStash.IsCheckedChanged += OnStashCheckedChanged;
		_dpiBindings.BindImage(_chkStash, Icons.Stash);

		var noMargin = DpiBoundValue.Constant(Padding.Empty);
		_ = new ControlLayout(this)
		{
			Content = new Grid(
				padding: DpiBoundValue.Padding(new Padding(6, 2, 6, 2)),
				rows:
				[
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
				],
				content:
				[
					new GridContent(new ControlContent(_chkAlignToGraph.Control, marginOverride: noMargin), row: 0),
					new GridContent(new ControlContent(_grpVisibleReferences, marginOverride: DpiBoundValue.Padding(new(-3, 3, 0, 0))), row: 1),
					new GridContent(new ControlContent(_chkLocalBranches.Control, marginOverride: noMargin), row: 2),
					new GridContent(new ControlContent(_chkRemoteBranches.Control, marginOverride: noMargin), row: 3),
					new GridContent(new ControlContent(_chkTags.Control, marginOverride: noMargin), row: 4),
					new GridContent(new ControlContent(_chkStash.Control, marginOverride: noMargin), row: 5),
				]),
		};

		_chkAlignToGraph.Control.Parent = this;
		_grpVisibleReferences.Parent = this;
		_chkLocalBranches.Control.Parent = this;
		_chkRemoteBranches.Control.Parent = this;
		_chkTags.Control.Parent = this;
		_chkStash.Control.Parent = this;
	}

	private void SubscribeToColumnEvents()
	{
		Column.AlignToGraphChanged       += OnColumnAlignToGraphChanged;
		Column.ShowLocalBranchesChanged  += OnColumnShowLocalBranchesChanged;
		Column.ShowRemoteBranchesChanged += OnColumnShowRemoteBranchesChanged;
		Column.ShowTagsChanged           += OnColumnShowTagsChanged;
		Column.ShowStashChanged          += OnColumnShowStashChanged;
	}

	private void UnsubscribeFromColumnEvents()
	{
		Column.AlignToGraphChanged       -= OnColumnAlignToGraphChanged;
		Column.ShowLocalBranchesChanged  -= OnColumnShowLocalBranchesChanged;
		Column.ShowRemoteBranchesChanged -= OnColumnShowRemoteBranchesChanged;
		Column.ShowTagsChanged           -= OnColumnShowTagsChanged;
		Column.ShowStashChanged          -= OnColumnShowStashChanged;
	}

	public void UpdateStates()
	{
		_disableEvents = true;
		_chkAlignToGraph.IsChecked = Column.AlignToGraph;
		_chkLocalBranches.IsChecked = Column.ShowLocalBranches;
		_chkRemoteBranches.IsChecked = Column.ShowRemoteBranches;
		_chkTags.IsChecked = Column.ShowTags;
		_chkStash.IsChecked = Column.ShowStash;
		_disableEvents = false;
	}

	protected override void OnStyleChanged()
	{
		base.OnStyleChanged();
		CreateControls();
	}

	#endregion

	#region EVent Handlers

	private void OnColumnAlignToGraphChanged(object? sender, EventArgs e)
	{
		AlignToGraph = Column.AlignToGraph;
	}

	private void OnColumnShowLocalBranchesChanged(object? sender, EventArgs e)
	{
		ShowLocalBranches = Column.ShowLocalBranches;
	}

	private void OnColumnShowRemoteBranchesChanged(object? sender, EventArgs e)
	{
		ShowRemoteBranches = Column.ShowRemoteBranches;
	}

	private void OnColumnShowTagsChanged(object? sender, EventArgs e)
	{
		ShowTags = Column.ShowTags;
	}

	private void OnColumnShowStashChanged(object? sender, EventArgs e)
	{
		ShowStash = Column.ShowStash;
	}

	private void OnAlignToGraphCheckedChanged(object? sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_disableEvents = true;
			Column.AlignToGraph = ((ICheckBoxWidget)sender!).IsChecked;
			_disableEvents = false;
		}
	}

	private void OnLocalBranchesCheckedChanged(object? sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_disableEvents = true;
			Column.ShowLocalBranches = ((ICheckBoxWidget)sender!).IsChecked;
			_disableEvents = false;
		}
	}

	private void OnRemoteBranchesCheckedChanged(object? sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_disableEvents = true;
			Column.ShowRemoteBranches = ((ICheckBoxWidget)sender!).IsChecked;
			_disableEvents = false;
		}
	}

	private void OnTagsCheckedChanged(object? sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_disableEvents = true;
			Column.ShowTags = ((ICheckBoxWidget)sender!).IsChecked;
			_disableEvents = false;
		}
	}

	private void OnStashCheckedChanged(object? sender, EventArgs e)
	{
		if(!_disableEvents)
		{
			_disableEvents = true;
			Column.ShowStash = ((ICheckBoxWidget)sender!).IsChecked;
			_disableEvents = false;
		}
	}

	#endregion

	#region Properties

	public new SubjectColumn Column => (SubjectColumn)base.Column;

	public bool AlignToGraph
	{
		get => _chkAlignToGraph is not null ? _chkAlignToGraph.IsChecked : Column.AlignToGraph;
		private set
		{
			if(_chkAlignToGraph is not null)
			{
				_disableEvents = true;
				_chkAlignToGraph.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	public bool ShowLocalBranches
	{
		get => _chkLocalBranches is not null ? _chkLocalBranches.IsChecked : Column.ShowLocalBranches;
		private set
		{
			if(_chkLocalBranches is not null)
			{
				_disableEvents = true;
				_chkLocalBranches.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	public bool ShowRemoteBranches
	{
		get => _chkRemoteBranches is not null ? _chkRemoteBranches.IsChecked : Column.ShowRemoteBranches;
		private set
		{
			if(_chkRemoteBranches is not null)
			{
				_disableEvents = true;
				_chkRemoteBranches.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	public bool ShowTags
	{
		get => _chkTags is not null ? _chkTags.IsChecked : Column.ShowTags;
		private set
		{
			if(_chkTags is not null)
			{
				_disableEvents = true;
				_chkTags.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	public bool ShowStash
	{
		get => _chkStash is not null ? _chkStash.IsChecked : Column.ShowStash;
		private set
		{
			if(_chkStash is not null)
			{
				_disableEvents = true;
				_chkStash.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	#endregion
}
