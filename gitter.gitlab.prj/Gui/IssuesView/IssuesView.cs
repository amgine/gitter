#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;
using gitter.GitLab.Api;
using gitter.GitLab.Gui.ListBoxes;

using Resources = gitter.GitLab.Properties.Resources;

partial class IssuesView : GitLabViewBase, ISearchableView<IssuesSearchOptions>
{
	#region Data

	private readonly IssuesListBox _lstIssues;
	private readonly IssuesToolbar _toolbar;
	private ISearchToolBarController _searchToolbar;
	private IssuesListBinding? _dataSource;
	private IssueState? _issueState = Api.IssueState.Opened;
	private IssueScope? _issueScope;
	private HashSet<string> _selectedLabels = new(StringComparer.Ordinal);
	private string? _selectedMilestone;
	private LabelRegistry? _labelRegistry;

	#endregion

	#region .ctor

	public IssuesView(IWorkingEnvironment environment)
		: base(Guids.IssuesViewGuid, environment)
	{
		SuspendLayout();
		Name = nameof(IssuesView);
		Text   = Resources.StrIssues;
		_lstIssues = new()
		{
			Dock        = DockStyle.Fill,
			BorderStyle = BorderStyle.None,
			Text        = Resources.StrsNoIssuesToDisplay,
			Parent      = this,
		};
		Search = new IssuesSearch(_lstIssues);
		_searchToolbar = CreateSearchToolbarController<IssuesView, IssuesSearchToolBar, IssuesSearchOptions>(this);
		AddTopToolStrip(_toolbar = new(this));
		_lstIssues.ItemActivated += OnItemActivated;
		_lstIssues.PreviewKeyDown += OnKeyDown;
		ResumeLayout(performLayout: false);
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DataSource = null;
		}
		base.Dispose(disposing);
	}

	#endregion

	#region Properties

	public override IImageProvider ImageProvider { get; } = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"issues");

	private IssuesListBinding? DataSource
	{
		get => _dataSource;
		set
		{
			if(_dataSource == value) return;

			_dataSource?.Dispose();
			_dataSource = value;
			_dataSource?.ReloadData();
		}
	}

	public IssueState? IssueState
	{
		get => _issueState;
		set
		{
			if(_issueState == value) return;

			_issueState = value;
			if(DataSource is not null)
			{
				DataSource.IssueState = value;
			}
		}
	}

	public IssueScope? IssueScope
	{
		get => _issueScope;
		set
		{
			if(_issueScope == value) return;
			_issueScope = value;
			if(DataSource is not null) DataSource.IssueScope = value;
		}
	}

	public HashSet<string> SelectedLabels => _selectedLabels;

	public void SetSelectedLabels(IEnumerable<string> labels)
	{
		_selectedLabels = new HashSet<string>(labels, StringComparer.Ordinal);
		if(DataSource is not null)
		{
			DataSource.Labels = _selectedLabels.Count == 0 ? null : _selectedLabels;
		}
	}

	public string? SelectedMilestone
	{
		get => _selectedMilestone;
		set
		{
			if(string.Equals(_selectedMilestone, value, StringComparison.Ordinal)) return;
			_selectedMilestone = value;
			if(DataSource is not null) DataSource.Milestone = value;
		}
	}

	internal LabelRegistry? LabelRegistry => _labelRegistry;

	internal CustomListBoxItemsCollection IssuesListBoxItems => _lstIssues.Items;

	#endregion

	#region Methods

	protected override void OnContextDetached(GitLabServiceContext serviceContext)
	{
		DataSource = default;
		if(_labelRegistry is not null)
		{
			_labelRegistry.Dispose();
			_labelRegistry = null;
		}
	}

	protected override void OnContextAttached(GitLabServiceContext serviceContext)
	{
		_labelRegistry = new LabelRegistry(serviceContext);
		LabelRegistry.Current = _labelRegistry;
		_ = _labelRegistry.RefreshAsync();
		DataSource = new IssuesListBinding(serviceContext, _lstIssues, IssueState);
		if(_issueScope        is not null) DataSource.IssueScope = _issueScope;
		if(_selectedLabels.Count > 0)      DataSource.Labels     = _selectedLabels;
		if(_selectedMilestone is not null) DataSource.Milestone  = _selectedMilestone;
	}

	private void OnItemActivated(object? sender, ItemEventArgs e)
	{
		if(e.Item is IssueListItem item)
		{
			ShowIssueDetails(item.DataContext);
		}
	}

	private void ShowIssueDetails(Issue issue)
	{
		Assert.IsNotNull(issue);

		Utility.OpenUrl(issue.WebUrl);
	}

	protected override void SaveMoreViewTo(Section section)
	{
		var listNode = section.GetCreateSection("IssuesList");
		_lstIssues.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		var listNode = section.TryGetSection("IssuesList");
		if(listNode is not null)
		{
			_lstIssues.LoadViewFrom(listNode);
		}
	}

	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		OnKeyDown(this, e);
		base.OnPreviewKeyDown(e);
	}

	private void OnKeyDown(object? sender, PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.KeyCode)
		{
			case Keys.F when e.Modifiers == Keys.Control:
				_searchToolbar.Show();
				e.IsInputKey = true;
				break;
			case Keys.F5:
				RefreshContent();
				break;
		}
	}

	public void RefreshContent() => DataSource?.ReloadData();

	#endregion

	#region ISearchableView

	public ISearch<IssuesSearchOptions> Search { get; }

	public bool SearchToolBarVisible
	{
		get => _searchToolbar.IsVisible;
		set => _searchToolbar.IsVisible = value;
	}

	#endregion
}
