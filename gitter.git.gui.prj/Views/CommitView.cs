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
	using System.IO;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class CommitView : GitViewBase
	{
		private readonly CommitToolbar _toolbar;
		private TextBoxSpellChecker _speller;
		private bool _treeMode;
		private bool _suppressDiffUpdate;

		public CommitView(GuiProvider gui)
			: base(Guids.CommitViewGuid, gui)
		{
			InitializeComponent();

			Text = Resources.StrCommit;

			_treeMode = true;

			_splitContainer.BackColor = GitterApplication.Style.Colors.WorkArea;
			_splitContainer.Panel1.BackColor = GitterApplication.Style.Colors.Window;
			_splitContainer.Panel2.BackColor = GitterApplication.Style.Colors.Window;

			_lblStaged.Text = Resources.StrsStagedChanges.AddColon();
			_lstStaged.Text = Resources.StrsNoStagedChanges;
			_lblUnstaged.Text = Resources.StrsUnstagedChanges.AddColon();
			_lstUnstaged.Text = Resources.StrsNoUnstagedChanges;
			_lblMessage.Text = Resources.StrMessage.AddColon();
			_chkAmend.Text = Resources.StrAmend;
			_btnCommit.Text = Resources.StrCommit;

			_lstStaged.PreviewKeyDown += OnKeyDown;
			_lstUnstaged.PreviewKeyDown += OnKeyDown;
			_txtMessage.PreviewKeyDown += OnKeyDown;
			_chkAmend.Control.PreviewKeyDown += OnKeyDown;
			_btnCommit.Control.PreviewKeyDown += OnKeyDown;

			if(GitterApplication.Style.Type == GitterStyleType.DarkBackground)
			{
				_txtMessage.BorderStyle = BorderStyle.FixedSingle;
			}

			GitterApplication.FontManager.InputFont.Apply(_txtMessage);

			_lstStaged.Columns[0].SizeMode = ColumnSizeMode.Auto;
			_lstUnstaged.Columns[0].SizeMode = ColumnSizeMode.Auto;

			_lstStaged.ItemActivated += OnStagedItemActivated;
			_lstUnstaged.ItemActivated += OnUnstagedItemActivated;

			_lstStaged.GotFocus += OnStagedGotFocus;
			_lstUnstaged.GotFocus += OnUnstagedGotFocus;

			for(int i = 0; i < _lstUnstaged.Columns.Count; ++i)
			{
				var column = _lstUnstaged.Columns[i];
				column.IsVisible = column.Id == (int)ColumnId.Name;
			}

			for(int i = 0; i < _lstStaged.Columns.Count; ++i)
			{
				var column = _lstStaged.Columns[i];
				column.IsVisible = column.Id == (int)ColumnId.Name;
			}

			_txtMessage.BackColor = GitterApplication.Style.Colors.Window;
			_txtMessage.ForeColor = GitterApplication.Style.Colors.WindowText;

			AddTopToolStrip(_toolbar = new CommitToolbar(this));
			_speller = new TextBoxSpellChecker(_txtMessage, true);
		}

		private void LoadCommitMessage()
		{
			if(Repository != null)
			{
				Message = Repository.Status.LoadCommitMessage();
			}
		}

		private void SaveCommitMessage()
		{
			if(Repository != null)
			{
				Repository.Status.SaveCommitMessage(Message);
			}
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		protected override void AttachToRepository(Repository repository)
		{
			lock(repository.Status.SyncRoot)
			{
				_lstUnstaged.SetTree(Repository.Status.UnstagedRoot, _treeMode ?
					TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
				_lstStaged.SetTree(Repository.Status.StagedRoot, _treeMode ?
					TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
			}

			_lstStaged.SelectionChanged += OnSelectedStagedItemsChanged;
			_lstUnstaged.SelectionChanged += OnSelectedUnstagedItemsChanged;
			LoadCommitMessage();
		}

		protected override void DetachFromRepository(Repository repository)
		{
			SaveCommitMessage();
			_lstStaged.Clear();
			_lstUnstaged.Clear();
			_lstStaged.SelectionChanged -= OnSelectedStagedItemsChanged;
			_lstUnstaged.SelectionChanged -= OnSelectedUnstagedItemsChanged;
			_txtMessage.Text = string.Empty;
		}

		public bool TreeMode
		{
			get { return _treeMode; }
			set
			{
				if(_treeMode != value)
				{
					_treeMode = value;
					lock(Repository.Status.SyncRoot)
					{
						_lstUnstaged.SetTree(Repository.Status.UnstagedRoot, _treeMode ?
							TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
						_lstStaged.SetTree(Repository.Status.StagedRoot, _treeMode?
							TreeListBoxMode.ShowFullTree : TreeListBoxMode.ShowPlainFileList);
					}
				}
			}
		}

		public string Message
		{
			get { return _txtMessage.Text; }
			set { _txtMessage.Text = value; }
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgCommit"]; }
		}

		public override void OnActivated()
		{
			_chkAmend.Control.Enabled = !Repository.Head.IsEmpty;
		}

		protected override void OnClosing()
		{
			SaveCommitMessage();
			base.OnClosing();
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
						_lstStaged.BeginUpdate();
						_lstUnstaged.BeginUpdate();
						Repository.Status.Refresh();
						_lstStaged.EndUpdate();
						_lstUnstaged.EndUpdate();
					}
				}
			}
		}

		private void OnStagedGotFocus(object sender, EventArgs e)
		{
			if(_lstUnstaged.SelectedItems.Count != 0)
			{
				_suppressDiffUpdate = true;
				_lstUnstaged.SelectedItems.Clear();
				_suppressDiffUpdate = false;
				ShowContextualDiffView(null);
			}
		}

		private void OnUnstagedGotFocus(object sender, EventArgs e)
		{
			if(_lstStaged.SelectedItems.Count != 0)
			{
				_suppressDiffUpdate = true;
				_lstStaged.SelectedItems.Clear();
				_suppressDiffUpdate = false;
				ShowContextualDiffView(null);
			}
		}

		private void OnSelectedUnstagedItemsChanged(object sender, EventArgs e)
		{
			if(_suppressDiffUpdate) return;
			var items = ((CustomListBox)sender).SelectedItems;
			if(items.Count == 0)
			{
				ShowContextualDiffView(null);
			}
			else
			{
				var paths = new string[items.Count];
				for(int i = 0; i < paths.Length; ++i)
				{
					paths[i] = ((ITreeItemListItem)items[i]).TreeItem.RelativePath;
				}

				ShowContextualDiffView(Repository.Status.GetDiffSource(false, paths));
			}
		}

		private void OnSelectedStagedItemsChanged(object sender, EventArgs e)
		{
			if(_suppressDiffUpdate) return;
			var items = ((CustomListBox)sender).SelectedItems;
			if(items.Count == 0)
			{
				ShowContextualDiffView(null);
			}
			else
			{
				var paths = new string[items.Count];
				for(int i = 0; i < paths.Length; ++i)
				{
					paths[i] = ((ITreeItemListItem)items[i]).TreeItem.RelativePath;
				}

				ShowContextualDiffView(Repository.Status.GetDiffSource(true, paths));
			}
		}

		private void OnAmendCheckedChanged(object sender, EventArgs e)
		{
			if(_chkAmend.IsChecked)
			{
				if(_txtMessage.TextLength == 0)
				{
					var head = Repository.Head;
					if(head != null)
					{
						var rev = head.Revision;
						_txtMessage.AppendText(Utility.ExpandNewLineCharacters(rev.Subject));
						if(!string.IsNullOrEmpty(rev.Body))
						{
							_txtMessage.AppendText(Environment.NewLine);
							_txtMessage.AppendText(Environment.NewLine);
							_txtMessage.AppendText(Utility.ExpandNewLineCharacters(rev.Body));
						}
						_txtMessage.SelectAll();
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

		private void OnStagedItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as TreeFileListItem;
			if(item != null)
			{
				try
				{
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						item.DataContext.Unstage();
					}
				}
				catch(GitException exc)
				{
					GitterApplication.MessageBoxService.Show(
						this,
						exc.Message,
						Resources.ErrFailedToUnstage,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		private void OnUnstagedItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as TreeFileListItem;
			if(item != null)
			{
				try
				{
					using(this.ChangeCursor(Cursors.WaitCursor))
					{
						item.DataContext.Stage();
					}
				}
				catch(GitException exc)
				{
					GitterApplication.MessageBoxService.Show(
						this,
						exc.Message,
						Resources.ErrFailedToStage,
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		private void OnCommitClick(object sender, EventArgs e)
		{
			string message = _txtMessage.Text.Trim();
			bool amend = _chkAmend.Control.Enabled && _chkAmend.IsChecked;
			bool merging = Repository.State == RepositoryState.Merging;
			if(_lstStaged.Items.Count == 0 && !merging && !amend)
			{
				NotificationService.NotifyInputError(
					_lstStaged,
					Resources.ErrNothingToCommit,
					Resources.ErrNofilesStagedForCommit);
				return;
			}
			if(message.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtMessage,
					Resources.ErrEmptyCommitMessage,
					Resources.ErrEnterCommitMessage);
				return;
			}
			else if(message.Length < 2)
			{
				NotificationService.NotifyInputError(
					_txtMessage,
					Resources.ErrShortCommitMessage,
					Resources.ErrEnterLongerCommitMessage.UseAsFormat(2));
				return;
			}
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Repository.Status.Commit(message, amend);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToCommit,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return;
			}
			_txtMessage.Text = string.Empty;
			_chkAmend.IsChecked = false;
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);

			section.SetValue<bool>("TreeMode", _treeMode);
			var stagedListSection = section.GetCreateSection("StagedList");
			_lstStaged.SaveViewTo(stagedListSection);
			var unstagedListSection = section.GetCreateSection("UnstagedList");
			_lstUnstaged.SaveViewTo(unstagedListSection);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);

			_toolbar.TreeModeButton.Checked = _treeMode = section.GetValue<bool>("TreeMode", true);
			var stagedListSection = section.TryGetSection("StagedList");
			if(stagedListSection != null)
			{
				_lstStaged.LoadViewFrom(stagedListSection);
			}
			var unstagedListSection = section.TryGetSection("UnstagedList");
			if(unstagedListSection != null)
			{
				_lstUnstaged.LoadViewFrom(unstagedListSection);
			}
		}
	}
}
