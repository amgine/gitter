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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
sealed class ContributorsView : GitViewBase, ISearchableView<ContributorsSearchOptions>
{
	private readonly UsersListBox _lstUsers;
	private readonly ContributorsToolBar _toolbar;
	private readonly ISearchToolBarController _searchToolbar;

	public ContributorsView(GuiProvider gui)
		: base(Guids.ContributorsViewGuid, gui)
	{
		Name = nameof(ContributorsView);
		SuspendLayout();
		_lstUsers = new()
		{
			Name        = nameof(_lstUsers),
			BorderStyle = BorderStyle.None,
			Dock        = DockStyle.Fill,
			Margin      = Padding.Empty,
			TabIndex    = 0,
			Text        = "No Users",
			Parent      = this,
		};
		ResumeLayout(false);
		PerformLayout();

		Text   = Resources.StrContributors;
		Search = new ContributorsSearch(_lstUsers);

		_searchToolbar = CreateSearchToolbarController<ContributorsView, ContributorsSearchToolBar, ContributorsSearchOptions>(this);

		_lstUsers.Text = Resources.StrsNoContributorsToDisplay;
		_lstUsers.PreviewKeyDown += OnKeyDown;

		AddTopToolStrip(_toolbar = new ContributorsToolBar(this));
	}

	public override IImageProvider ImageProvider => Icons.User;

	protected override void AttachToRepository(Repository repository)
	{
		_lstUsers.Load(repository);
	}

	protected override void DetachFromRepository(Repository repository)
	{
		_lstUsers.Load(null);
	}

	public override void RefreshContent()
	{
		if(InvokeRequired)
		{
			BeginInvoke(new MethodInvoker(RefreshContent));
		}
		else
		{
			if(Repository is not null)
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Repository.Users.Refresh();
				}
			}
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
				e.IsInputKey = true;
				break;
		}
	}

	protected override void SaveMoreViewTo(Section section)
	{
		base.SaveMoreViewTo(section);
		var listNode = section.GetCreateSection("UsersList");
		_lstUsers.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);
		var listNode = section.TryGetSection("UsersList");
		if(listNode is not null)
		{
			_lstUsers.LoadViewFrom(listNode);
		}
	}

	public ISearch<ContributorsSearchOptions> Search { get; }

	public bool SearchToolBarVisible
	{
		get => _searchToolbar.IsVisible;
		set => _searchToolbar.IsVisible = value;
	}
}
