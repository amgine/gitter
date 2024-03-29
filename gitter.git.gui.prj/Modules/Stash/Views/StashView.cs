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
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
partial class StashView : GitViewBase
{
	private readonly StashListBox _lstStash;
	private readonly StashToolbar _toolBar;

	public StashView(GuiProvider gui, IFactory<StashListBox> stashListBoxFactory)
		: base(Guids.StashViewGuid, gui)
	{
		Verify.Argument.IsNotNull(stashListBoxFactory);

		SuspendLayout();
		_lstStash = stashListBoxFactory.Create();
		_lstStash.Name        = nameof(_lstStash);
		_lstStash.BorderStyle = BorderStyle.None;
		_lstStash.Dock        = DockStyle.Fill;
		_lstStash.TabIndex    = 0;
		_lstStash.Parent      = this;
		Name = nameof(StashView);
		ResumeLayout(false);
		PerformLayout();

		Text = Resources.StrStash;

		_lstStash.Text = Resources.StrsNothingStashed;
		_lstStash.PreviewKeyDown += OnKeyDown;

		AddTopToolStrip(_toolBar = new StashToolbar(this));
	}

	private static readonly IDpiBoundValue<Size> _defaultScalableSize = DpiBoundValue.Size(new(555, 160));

	public override IDpiBoundValue<Size> DefaultScalableSize => _defaultScalableSize;

	protected override void AttachToRepository(Repository repository)
	{
		_lstStash.LoadData(repository);
	}

	protected override void DetachFromRepository(Repository repository)
	{
		_lstStash.LoadData(null);
	}

	public override IImageProvider ImageProvider => Icons.Stash;

	public override void RefreshContent()
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			catch
			{
			}
		}
		else
		{
			if(Repository is not null)
			{
				Repository.Stash.Refresh();
			}
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
			case Keys.F5:
				RefreshContent();
				e.IsInputKey = true;
				break;
		}
	}

	protected override void SaveMoreViewTo(Section section)
	{
		base.SaveMoreViewTo(section);
		var listNode = section.GetCreateSection("StashList");
		_lstStash.SaveViewTo(listNode);
	}

	protected override void LoadMoreViewFrom(Section section)
	{
		base.LoadMoreViewFrom(section);
		var listNode = section.TryGetSection("StashList");
		if(listNode is not null)
		{
			_lstStash.LoadViewFrom(listNode);
		}
	}
}
