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
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Configuration;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;

	using Resources = Properties.Resources;

	/// <summary>Tool for displaying a sequence of revisions with graph support.</summary>
	[ToolboxItem(false)]
	partial class HistoryView : HistoryViewBase
	{
		private readonly HistoryToolbar _toolbar;
		private IDisposable _urlHandler;

		public HistoryView(GuiProvider gui)
			: base(Guids.HistoryViewGuid, gui)
		{
			RevisionListBox.PreviewKeyDown += OnKeyDown;
			Text = Resources.StrHistory;
			AddTopToolStrip(_toolbar = new HistoryToolbar(this));

			_urlHandler = Hyperlink.RegisterInternalHandler(OnNavigate);
		}

		protected override void Dispose(bool disposing)
		{
			if(_urlHandler is not null)
			{
				_urlHandler.Dispose();
				_urlHandler = null;
			}
			base.Dispose(disposing);
		}

		private bool OnNavigate(string url)
		{
			const string prefix = @"gitter://history/";

			if(!url.StartsWith(prefix)) return false;
			if(Repository is null) return false;

			try
			{
				var ptr = Repository.GetRevisionPointer(url.Substring(prefix.Length));
				return SelectRevision(ptr);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				return false;
			}
		}

		/// <inheritdoc/>
		public override IImageProvider ImageProvider { get; } = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"history");

		protected override void AttachToRepository(Repository repository)
		{
			Assert.IsNotNull(repository);

			base.AttachToRepository(repository);

			repository.CommitCreated += OnCommitCreated;
			repository.Updated += OnRepositoryUpdated;
			repository.Stash.StashedStateDeleted += OnStashDeleted;

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
			if(logOptionsNode is not null)
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
				if(item is not null)
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
				try
				{
					BeginInvoke(new MethodInvoker(RefreshContentSync));
				}
				catch(ObjectDisposedException)
				{
				}
			}
			else
			{
				RefreshContentSync();
			}
		}

		private void RefreshContentSync()
		{
			if(IsDisposed) return;
			if(Repository is not null && LogSource is not null)
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
				_toolbar.ShowDiffButton.Checked = ShowDetails = layoutNode.GetValue("ShowDetails", ShowDetails);
			}
			var listNode = section.TryGetSection("RevisionList");
			if(listNode is not null)
			{
				RevisionListBox.LoadViewFrom(listNode);
			}
		}
	}
}
