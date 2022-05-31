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
partial class ConfigView : GitViewBase, ISearchableView<ConfigSearchOptions>
{
	private readonly ConfigListBox _lstConfig;
	private readonly ConfigToolBar _toolBar;
	private readonly ISearchToolBarController _searchToolbar;

	public ConfigView(GuiProvider gui)
		: base(Guids.ConfigViewGuid, gui)
	{
		SuspendLayout();
		Name = nameof(ConfigView);
		_lstConfig = new()
		{
			BorderStyle = BorderStyle.None,
			Dock        = DockStyle.Fill,
			Name        = nameof(_lstConfig),
			TabIndex    = 1,
			Text        = "No parameters found",
			Parent      = this,
		};
		ResumeLayout(false);
		PerformLayout();

		_lstConfig.PreviewKeyDown += OnKeyDown;

		Text   = Resources.StrConfig;
		Search = new ConfigSearch(_lstConfig);

		_searchToolbar = CreateSearchToolbarController<ConfigView, ConfigSearchToolBar, ConfigSearchOptions>(this);

		AddTopToolStrip(_toolBar = new ConfigToolBar(this));
	}

	/// <inheritdoc/>
	public override bool IsDocument => true;

	/// <inheritdoc/>
	protected override void AttachToRepository(Repository repository)
	{
		_lstConfig.LoadData(repository);
	}

	/// <inheritdoc/>
	protected override void DetachFromRepository(Repository repository)
	{
		_lstConfig.Clear();
	}

	/// <inheritdoc/>
	public override IImageProvider ImageProvider => Icons.Configuration;

	/// <inheritdoc/>
	public override void RefreshContent()
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			catch(ObjectDisposedException)
			{
			}
		}
		else
		{
			if(Repository is not null)
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Repository.Configuration.Refresh();
				}
			}
		}
	}

	/// <inheritdoc/>
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
			case Keys.F5 when e.Modifiers == Keys.None:
				RefreshContent();
				break;
		}
	}

	/// <inheritdoc/>
	protected override void SaveMoreViewTo(Section section)
	{
		base.SaveMoreViewTo(section);
		var listSection = section.GetCreateSection("ConfigParameterList");
		_lstConfig.SaveViewTo(listSection);
	}

	/// <inheritdoc/>
	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);
		var listSection = section.TryGetSection("ConfigParameterList");
		if(listSection is not null)
		{
			_lstConfig.LoadViewFrom(listSection);
		}
	}

	public ISearch<ConfigSearchOptions> Search { get; }

	public bool SearchToolBarVisible
	{
		get => _searchToolbar.IsVisible;
		set => _searchToolbar.IsVisible = value;
	}
}
