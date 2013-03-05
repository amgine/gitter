namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class CleanDialog : DialogBase, IExecutableDialog
	{
		#region Data

		private readonly Repository _repository;
		private IAsyncResult _asyncResult;
		private IAsyncFunc<IList<TreeItem>> _currentRequest;
		private readonly object _requestLock = new object();

		#endregion

		/// <summary>Create <see cref="CleanDialog"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		public CleanDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrClean;

			_lblIncludePattern.Text = Resources.StrIncludePattern.AddColon();
			_lblExcludePattern.Text = Resources.StrExcludePattern.AddColon();

			_lblType.Text = Resources.StrType.AddColon();

			_radIncludeUntracked.Text = Resources.StrUntracked;
			_radIncludeIgnored.Text = Resources.StrIgnored;
			_radIncludeBoth.Text = Resources.StrBoth;

			_chkRemoveDirectories.Text = Resources.StrsAlsoRemoveDirectories;

			_lblObjectList.Text = Resources.StrsObjectsThatWillBeRemoved.AddColon();
			_lstFilesToClear.Text = Resources.StrsNoFilesToRemove;


			for(int i = 0; i < _lstFilesToClear.Columns.Count; ++i)
			{
				var col = _lstFilesToClear.Columns[i];
				col.IsVisible = col.Id == (int)ColumnId.Name;
			}

			_lstFilesToClear.Style = GitterApplication.DefaultStyle;
			_lstFilesToClear.Columns[0].SizeMode = Framework.Controls.ColumnSizeMode.Auto;
			_lstFilesToClear.ShowTreeLines = false;

			if(!GitFeatures.CleanExcludeOption.IsAvailableFor(repository))
			{
				_lblExcludePattern.Enabled = false;
				_txtExclude.Enabled = false;
				_txtExclude.Text = Resources.StrlRequiredVersionIs.UseAsFormat(
					GitFeatures.CleanExcludeOption.RequiredVersion);
			}

			GitterApplication.FontManager.InputFont.Apply(_txtPattern, _txtExclude);

			LoadConfig();
		}

		protected override void OnClosed(DialogResult result)
		{
			SaveConfig();
			base.OnClosed(result);
		}

		private void LoadConfig()
		{
			var section = _repository.ConfigSection.TryGetSection("CleanDialog");
			if(section != null)
			{
				_txtPattern.Text = section.GetValue<string>("Pattern", string.Empty);
				_txtExclude.Text = section.GetValue<string>("Exclude", string.Empty);
				RemoveDirectories = section.GetValue<bool>("RemoveDirectories", false);
				Mode = section.GetValue<CleanFilesMode>("Mode", CleanFilesMode.Default);
			}
		}

		private void SaveConfig()
		{
			var section = _repository.ConfigSection.GetCreateSection("CleanDialog");
			section.SetValue<string>("Pattern", _txtPattern.Text);
			section.SetValue<string>("Exclude", _txtExclude.Text);
			section.SetValue<bool>("RemoveDirectories", RemoveDirectories);
			section.SetValue<CleanFilesMode>("Mode", Mode);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrClean; }
		}

		protected override void OnShown()
		{
			base.OnShown();
			UpdateList();
		}

		public CleanFilesMode Mode
		{
			get
			{
				if(_radIncludeUntracked.Checked)
					return CleanFilesMode.Default;
				if(_radIncludeIgnored.Checked)
					return CleanFilesMode.OnlyIgnored;
				if(_radIncludeBoth.Checked)
					return CleanFilesMode.IncludeIgnored;
				return CleanFilesMode.Default;
			}
			set
			{
				switch(value)
				{
					case CleanFilesMode.Default:
						_radIncludeUntracked.Checked = true;
						break;
					case CleanFilesMode.OnlyIgnored:
						_radIncludeIgnored.Checked = true;
						break;
					case CleanFilesMode.IncludeIgnored:
						_radIncludeBoth.Checked = true;
						break;
					default:
						throw new ArgumentException("value");
				}
			}
		}

		public bool RemoveDirectories
		{
			get { return _chkRemoveDirectories.Checked; }
			set { _chkRemoveDirectories.Checked = value; }
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
			var func = _repository.Status.GetFilesToCleanAsync(
				_txtPattern.Text.Trim(),
				_txtExclude.Text.Trim(),
				Mode,
				RemoveDirectories);
			lock(_requestLock)
			{
				_currentRequest = func;
				_asyncResult = func.BeginInvoke(this,
					_lstFilesToClear.ProgressMonitor, OnSearchCompleted, func);
			}
		}

		private void OnSearchCompleted(IAsyncResult ar)
		{
			var func = (IAsyncFunc<IList<TreeItem>>)ar.AsyncState;
			if(func != null)
			{
				var items = func.EndInvoke(ar);
				if(_lstFilesToClear.IsHandleCreated)
				{
					if(func == _currentRequest)
					{
						try
						{
							if(InvokeRequired)
							{
								var action = new Action<IList<TreeItem>>(UpdateFileList);
								BeginInvoke(action, items);
							}
							else
							{
								UpdateFileList(items);
							}
						}
						catch
						{
						}
					}
				}
			}
			lock(_requestLock)
			{
				_currentRequest = null;
				_asyncResult = null;
			}
		}

		private void UpdateFileList(IList<TreeItem> items)
		{
			_lstFilesToClear.BeginUpdate();
			_lstFilesToClear.Items.Clear();
			foreach(var item in items)
			{
				if(item.ItemType == TreeItemType.Tree)
				{
					_lstFilesToClear.Items.Add(new TreeDirectoryListItem(
						(TreeDirectory)item, TreeDirectoryListItemType.ShowNothing));
				}
				else
				{
					_lstFilesToClear.Items.Add(new TreeFileListItem(
						(TreeFile)item, true));
				}
			}
			_lstFilesToClear.EndUpdate();
		}

		private void OnPatternTextChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void OnRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			if(((RadioButton)sender).Checked)
			{
				UpdateList();
			}
		}

		private void _chkRemoveDirectories_CheckedChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void _lstFilesToClear_ItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as ITreeItemListItem;
			if(item != null)
			{
				Utility.OpenUrl(System.IO.Path.Combine(
					item.TreeItem.Repository.WorkingDirectory, item.TreeItem.RelativePath));
			}
		}

		/// <summary>Execute dialog associated action.</summary>
		/// <returns><c>true</c>, if action succeded</returns>
		public bool Execute()
		{
			var mode = Mode;
			string include = _txtPattern.Text.Trim();
			string exclude = _txtExclude.Enabled?
				_txtExclude.Text.Trim() : string.Empty;
			bool removeDirectories = RemoveDirectories;
			try
			{
				_repository.Status.Clean(include, exclude, mode, removeDirectories);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrCleanFailed,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				UpdateList();
				return false;
			}
			return true;
		}
	}
}
