﻿#region Copyright Notice
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
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;
using gitter.Framework.Controls;
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
sealed class SubmodulesView : GitViewBase, ISearchableView<SubmodulesSearchOptions>
{
	private readonly SubmodulesListBox _lstSubmodules;
	private readonly SubmodulesToolbar _toolBar;
	private ISearchToolBarController _searchToolbar;

	public SubmodulesView(GuiProvider gui, IFactory<SubmodulesListBox> submodulesListBoxFactory)
		: base(Guids.SubmodulesViewGuid, gui)
	{
		Verify.Argument.IsNotNull(submodulesListBoxFactory);

		SuspendLayout();
		_lstSubmodules = submodulesListBoxFactory.Create();
		_lstSubmodules.Name        = nameof(_lstSubmodules);
		_lstSubmodules.BorderStyle = BorderStyle.None;
		_lstSubmodules.Dock        = DockStyle.Fill;
		_lstSubmodules.Margin      = Padding.Empty;
		_lstSubmodules.Text        = Resources.StrsNoSubmodules;
		_lstSubmodules.Parent      = this;
		Name = nameof(SubmodulesView);
		Size = new(555, 160);
		ResumeLayout(false);
		PerformLayout();

		Text = Resources.StrSubmodules;
		Search = new SubmodulesSearch(_lstSubmodules);

		_searchToolbar = CreateSearchToolbarController<SubmodulesView, SubmodulesSearchToolBar, SubmodulesSearchOptions>(this);

		_lstSubmodules.PreviewKeyDown += OnKeyDown;

		AddTopToolStrip(_toolBar = new SubmodulesToolbar(this));
	}

	private static readonly IDpiBoundValue<Size> _defaultScalableSize = DpiBoundValue.Size(new(555, 160));

	public override IDpiBoundValue<Size> DefaultScalableSize => _defaultScalableSize;

	protected override void AttachToRepository(Repository repository)
	{
		_lstSubmodules.Load(repository);
		base.AttachToRepository(repository);
	}

	protected override void DetachFromRepository(Repository repository)
	{
		_lstSubmodules.Load(null);
		base.DetachFromRepository(repository);
	}

	public override IImageProvider ImageProvider => Icons.Submodule;

	public ISearch<SubmodulesSearchOptions> Search { get; }

	public bool SearchToolBarVisible
	{
		get => _searchToolbar.IsVisible;
		set => _searchToolbar.IsVisible = value;
	}

	public override void RefreshContent()
	{
		if(Repository is not null)
		{
			Repository.Submodules.Refresh();
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
		var listNode = section.GetCreateSection("SubmodulesList");
		_lstSubmodules.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);
		var listNode = section.TryGetSection("SubmodulesList");
		if(listNode is not null)
		{
			_lstSubmodules.LoadViewFrom(listNode);
		}
	}
}
