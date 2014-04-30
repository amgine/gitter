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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>Tool for displaying a sequence of revisions with graph support.</summary>
	[ToolboxItem(false)]
	partial class HistoryView : HistoryViewBase
	{
		#region Data

		private readonly HistoryToolbar _toolbar;

		#endregion

		#region .ctor

		public HistoryView(GuiProvider gui)
			: base(Guids.HistoryViewGuid, gui)
		{
			RevisionListBox.PreviewKeyDown += OnKeyDown;
			Text = Resources.StrHistory;
			AddTopToolStrip(_toolbar = new HistoryToolbar(this));
		}

		#endregion

		/// <summary>Gets view image.</summary>
		/// <value>This view image.</value>
		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgHistory"]; }
		}

		protected override void AttachToRepository(Repository repository)
		{
			Assert.IsNotNull(repository);

			base.AttachToRepository(repository);

			Repository.CommitCreated += OnCommitCreated;
			Repository.Updated += OnRepositoryUpdated;
			Repository.Stash.StashedStateDeleted += OnStashDeleted;

			LogSource = new RepositoryLogSource(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			Assert.IsNotNull(repository);

			base.DetachFromRepository(repository);

			LogSource = null;

			repository.CommitCreated -= OnCommitCreated;
			repository.Updated -= OnRepositoryUpdated;
			repository.Stash.StashedStateDeleted -= OnStashDeleted;
	
			LogOptions.Reset();
		}

		protected override void LoadRepositoryConfig(Section section)
		{
			base.LoadRepositoryConfig(section);
			var logOptionsNode = section.TryGetSection("LogOptions");
			if(logOptionsNode != null)
			{
				LogOptions.LoadFrom(logOptionsNode);
			}
		}

		protected override void SaveRepositoryConfig(Section section)
		{
			base.SaveRepositoryConfig(section);
			var logOptionsNode = section.GetCreateSection("LogOptions");
			LogOptions.SaveTo(logOptionsNode);
		}

		private void OnCommitCreated(object sender, RevisionEventArgs e)
		{
			RefreshContent();
		}

		private void OnRepositoryUpdated(object sender, EventArgs e)
		{
			RefreshContent();
		}

		private void OnStashDeleted(object sender, StashedStateEventArgs e)
		{
			if(e.Object.Index == 0)
			{
				var item = RevisionListBox.TryGetItem(e.Object.Revision);
				if(item != null)
				{
					RefreshContent();
				}
			}
		}

		/// <summary>Refreshes the content.</summary>
		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(RefreshContentSync));
			}
			else
			{
				RefreshContentSync();
			}
		}

		private void RefreshContentSync()
		{
			if(Repository != null && LogSource != null)
			{
				Repository.Status.Refresh();
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
			switch(e.KeyCode)
			{
				case Keys.F:
					if(e.Modifiers == Keys.Control)
					{
						ShowSearchToolBar();
						e.IsInputKey = true;
					}
					break;
				case Keys.F5:
					if(e.Modifiers == Keys.None)
					{
						RefreshContent();
					}
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
			if(layoutNode != null)
			{
				_toolbar.ShowDiffButton.Checked = ShowDetails = layoutNode.GetValue("ShowDetails", ShowDetails);
			}
			var listNode = section.TryGetSection("RevisionList");
			if(listNode != null)
			{
				RevisionListBox.LoadViewFrom(listNode);
			}
		}
	}
}
