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

	private readonly IssuesToolbar _toolbar;
	private ISearchToolBarController _searchToolbar;
	private IssuesListBinding _dataSource;
	private IssueState? _issueState = Api.IssueState.Opened;

	#endregion

	#region .ctor

	public IssuesView(IWorkingEnvironment environment)
		: base(Guids.IssuesViewGuid, environment)
	{
		InitializeComponent();
		_lstIssues.Text = Resources.StrsNoIssuesToDisplay;

		Text   = Resources.StrIssues;
		Search = new IssuesSearch(_lstIssues);

		_searchToolbar = CreateSearchToolbarController<IssuesView, IssuesSearchToolBar, IssuesSearchOptions>(this);

		AddTopToolStrip(_toolbar = new(this));

		_lstIssues.ItemActivated += OnItemActivated;
		_lstIssues.PreviewKeyDown += OnKeyDown;
	}

	#endregion

	#region Properties

	public override IImageProvider ImageProvider { get; } = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"issues");

	private IssuesListBinding DataSource
	{
		get => _dataSource;
		set
		{
			if(_dataSource != value)
			{
				_dataSource?.Dispose();
				_dataSource = value;
				_dataSource?.ReloadData();
			}
		}
	}

	public IssueState? IssueState
	{
		get => _issueState;
		set
		{
			if(_issueState != value)
			{
				_issueState = value;
				if(DataSource is not null)
				{
					DataSource.IssueState = value;
				}
			}
		}
	}

	#endregion

	#region Methods

	protected override void OnContextDetached(GitLabServiceContext serviceContext)
	{
		DataSource = default;
	}

	protected override void OnContextAttached(GitLabServiceContext serviceContext)
	{
		DataSource = new IssuesListBinding(serviceContext, _lstIssues, IssueState);
	}

	private void OnItemActivated(object sender, ItemEventArgs e)
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

	private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
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
