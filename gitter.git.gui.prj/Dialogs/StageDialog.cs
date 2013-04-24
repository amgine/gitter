namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;
	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class StageDialog : DialogBase, IExecutableDialog
	{
		private readonly Repository _repository;
		private IAsyncResult _asyncResult;
		private IAsyncFunc<IList<TreeFile>> _currentRequest;
		private readonly object _requestLock = new object();

		/// <summary>Create <see cref="StageDialog"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		public StageDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrStageFiles;

			_lblPattern.Text = Resources.StrPattern.AddColon();
			_chkIncludeUntracked.Text = Resources.StrIncludeUntracked;
			_chkIncludeIgnored.Text = Resources.StrIncludeIgnored;
			_lstUnstaged.Style = GitterApplication.DefaultStyle;
			_lstUnstaged.Text = Resources.StrsNoUnstagedChanges;

			for(int i = 0; i < _lstUnstaged.Columns.Count; ++i)
			{
				var col = _lstUnstaged.Columns[i];
				col.IsVisible = col.Id == (int)ColumnId.Name;
			}
			_lstUnstaged.Columns[0].SizeMode = Framework.Controls.ColumnSizeMode.Auto;
			_lstUnstaged.ShowTreeLines = false;

			GitterApplication.FontManager.InputFont.Apply(_txtPattern);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrStage; }
		}

		protected override void OnShown()
		{
			base.OnShown();
			UpdateList();
		}

		private void UpdateList()
		{
			lock(_requestLock)
			{
				var req = _currentRequest;
				var ar = _asyncResult;
				if(req != null && ar != null)
				{
					req.EndInvoke(ar);
				}
			}
			var func = _repository.Status.GetFilesToAddAsync(
				_txtPattern.Text.Trim(),
				_chkIncludeUntracked.Checked,
				_chkIncludeIgnored.Checked);
			lock(_requestLock)
			{
				_currentRequest = func;
				_asyncResult = func.BeginInvoke(this, _lstUnstaged.ProgressMonitor, OnSearchCompleted, func);
			}
		}

		private void OnSearchCompleted(IAsyncResult ar)
		{
			var func = (IAsyncFunc<IList<TreeFile>>)ar.AsyncState;
			var files = func.EndInvoke(ar);
			if(_lstUnstaged.IsHandleCreated)
			{
				if(func == _currentRequest)
				{
					try
					{
						if(InvokeRequired)
						{
							BeginInvoke(new Action<IList<TreeFile>>(UpdateFileList), files);
						}
						else
						{
							UpdateFileList(files);
						}
					}
					catch { }
				}
			}
			lock(_requestLock)
			{
				_currentRequest = null;
				_asyncResult = null;
			}
		}

		private void UpdateFileList(IList<TreeFile> files)
		{
			_lstUnstaged.BeginUpdate();
			_lstUnstaged.Items.Clear();
			foreach(var file in files)
			{
				_lstUnstaged.Items.Add(new TreeFileListItem(file, true));
			}
			_lstUnstaged.EndUpdate();
		}

		private void OnPatternTextChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void _chkIncludeUntracked_CheckedChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void _chkIncludeIgnored_CheckedChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void _lstFiles_ItemActivated(object sender, Framework.Controls.ItemEventArgs e)
		{
			var item = e.Item as ITreeItemListItem;
			if(item.TreeItem.Status != FileStatus.Removed)
			{
				if(item != null) Utility.OpenUrl(System.IO.Path.Combine(
					item.TreeItem.Repository.WorkingDirectory, item.TreeItem.RelativePath));
			}
		}

		public bool Execute()
		{
			try
			{
				if(_lstUnstaged.Items.Count == 0) return true;
				var pattern = _txtPattern.Text.Trim();
				bool addIgnored = _chkIncludeIgnored.Checked;
				bool addUntracked = _chkIncludeUntracked.Checked;
				_repository.Status.Stage(pattern, addUntracked, addIgnored);
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
			return true;
		}
	}
}
