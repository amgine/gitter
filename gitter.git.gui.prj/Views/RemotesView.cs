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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class RemotesView : GitViewBase
	{
		private RemotesToolbar _toolbar;

		public RemotesView(GuiProvider gui)
			: base(Guids.RemotesViewGuid, gui)
		{
			InitializeComponent();

			Text = Resources.StrRemotes;

			_lstRemotes.Text = Resources.StrsNoRemotes;

			_lstRemotes.ItemActivated += OnItemActivated;
			_lstRemotes.PreviewKeyDown += OnKeyDown;

			AddTopToolStrip(_toolbar = new RemotesToolbar(this));
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var remote = ((CustomListBoxItem<Remote>)e.Item).DataContext;
			Gui.Environment.ViewDockService.ShowView(Guids.RemoteViewGuid, new RemoteViewModel(remote), true);
		}

		protected override void AttachToRepository(Repository repository)
		{
			_lstRemotes.LoadData(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstRemotes.LoadData(null);
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgRemote"]; }
		}

		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			else
			{
				if(Repository != null)
				{
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						Repository.Remotes.Refresh();
					}
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
			var listNode = section.GetCreateSection("RemoteList");
			_lstRemotes.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("RemoteList");
			if(listNode != null)
			{
				_lstRemotes.LoadViewFrom(listNode);
			}
		}
	}
}
