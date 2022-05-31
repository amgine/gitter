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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Configuration;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
partial class PathHistoryView : HistoryViewBase
{
	private readonly PathHistoryToolbar _toolBar;

	public PathHistoryView(GuiProvider gui, IFactory<RevisionListBox> revisionListBoxFactory)
		: base(Guids.PathHistoryViewGuid, gui, revisionListBoxFactory)
	{
		RevisionListBox.PreviewKeyDown += OnKeyDown;
		AddTopToolStrip(_toolBar = new PathHistoryToolbar(this));
	}

	protected override void AttachViewModel(object viewModel)
	{
		base.AttachViewModel(viewModel);

		if(viewModel is HistoryViewModel vm)
		{
			LogSource = vm.LogSource as PathLogSource;
			Text = LogSource is not null
				? Resources.StrHistory + ": " + LogSource.ToString()
				: Resources.StrHistory;
		}
	}

	protected override void DetachViewModel(object viewModel)
	{
		base.DetachViewModel(viewModel);

		if(viewModel is HistoryViewModel)
		{
			LogSource = null;
			Text = Resources.StrHistory;
		}
	}

	protected new PathLogSource LogSource
	{
		get => (PathLogSource)base.LogSource;
		set => base.LogSource = value;
	}

	public override IImageProvider ImageProvider
		=> LogSource is not null && LogSource.Path.EndsWith('/')
			? Icons.FolderHistory
			: Icons.FileHistory;

	protected override void DetachFromRepository(Repository repository)
	{
		base.DetachFromRepository(repository);

		LogSource = null;
		LogOptions.Reset();
	}

	/// <summary>Refreshes the content.</summary>
	public override void RefreshContent()
	{
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(ReloadRevisionLog));
			}
			catch
			{
			}
		}
		else
		{
			ReloadRevisionLog();
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
			case Keys.F5 when e.Modifiers == Keys.None:
				RefreshContent();
				break;
		}
	}

	protected override void SaveMoreViewTo(Section section)
	{
		base.SaveMoreViewTo(section);
		var layoutNode = section.GetCreateSection("Layout");
		layoutNode.SetValue("ShowDetails", ShowDetails);
		var listNode = section.GetCreateSection("RevisionList");
		RevisionListBox.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);
		var layoutNode = section.TryGetSection("Layout");
		if(layoutNode is not null)
		{
			//_toolbar.ShowDiffButton.Checked = ShowDetails = layoutNode.GetValue("ShowDetails", ShowDetails);
		}
		var listNode = section.TryGetSection("RevisionList");
		if(listNode is not null)
		{
			RevisionListBox.LoadViewFrom(listNode);
		}
	}
}
