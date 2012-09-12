namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;

	using gitter.Git.AccessLayer;
	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public partial class ApplyPatchesDialog : GitDialogBase, IExecutableDialog
	{
		private Repository _repository;

		public ApplyPatchesDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrApplyPatches;

			_lstPatches.HeaderStyle = Framework.Controls.HeaderStyle.Hidden;
			_lstPatches.ShowCheckBoxes = true;
			_lstPatches.Multiselect = true;
			_lstPatches.Columns.Add(new NameColumn() { SizeMode = Framework.Controls.ColumnSizeMode.Auto });
			_lstPatches.KeyDown += OnPatchesKeyDown;

			_lblPatches.Text = Resources.StrPatches.AddColon();
			_lstPatches.Text = Resources.StrsNoPatchesToApply;
			_btnAddFiles.Text = Resources.StrAddFiles.AddEllipsis();
			_btnAddFromClipboard.Text = Resources.StrAddFromClipboard;
			_grpApplyTo.Text = Resources.StrApplyTo;
			_radWorkingDirectory.Text = Resources.StrWorkingDirectory;
			_radIndexOnly.Text = Resources.StrIndex;
			_radIndexAndWorkingDirectory.Text = Resources.StrIndexAndWorkingDirectory;
			_grpOptions.Text = Resources.StrOptions;
			_chkReverse.Text = Resources.StrReverse;
		}

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		public ApplyPatchTo ApplyTo
		{
			get
			{
				if(_radWorkingDirectory.Checked)
				{
					return ApplyPatchTo.WorkingDirectory;
				}
				if(_radIndexOnly.Checked)
				{
					return ApplyPatchTo.Index;
				}
				if(_radIndexAndWorkingDirectory.Checked)
				{
					return ApplyPatchTo.IndexAndWorkingDirectory;
				}
				return ApplyPatchTo.WorkingDirectory;
			}
			set
			{
				switch(value)
				{
					case ApplyPatchTo.WorkingDirectory:
						_radWorkingDirectory.Checked = true;
						break;
					case ApplyPatchTo.Index:
						_radIndexOnly.Checked = true;
						break;
					case ApplyPatchTo.IndexAndWorkingDirectory:
						_radIndexAndWorkingDirectory.Checked = true;
						break;
					default:
						throw new ArgumentException();
				}
			}
		}

		public bool Reverse
		{
			get { return _chkReverse.Checked; }
			set { _chkReverse.Checked = value; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrApply; }
		}

		public IEnumerable<IPatchSource> SelectedPatchSources
		{
			get
			{
				foreach(PatchSourceListItem item in _lstPatches.Items)
				{
					if(item.IsChecked)
					{
						yield return item.DataContext;
					}
				}
			}
		}

		#endregion

		#region Event Handlers

		private void OnPatchesKeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Delete:
					while(_lstPatches.SelectedItems.Count != 0)
					{
						_lstPatches.SelectedItems[0].Remove();
					}
					break;
			}
		}

		private void OnAddFilesClick(object sender, EventArgs e)
		{
			using(var dlg = new OpenFileDialog()
				{
					Filter = "Patches (*.patch)|*.patch|All files|*.*",
					Multiselect = true,
				})
			{
				if(dlg.ShowDialog(this) == DialogResult.OK)
				{
					foreach(var fileName in dlg.FileNames)
					{
						var src = new PatchFromFile(fileName);
						AddPatchSource(src);
					}
				}
			}
		}

		private void OnAddFromClipboardClick(object sender, EventArgs e)
		{
			string patch = null;
			try
			{
				patch = Clipboard.GetText();
			}
			catch
			{
			}
			if(!string.IsNullOrWhiteSpace(patch))
			{
				var src = new PatchFromString("Patch from clipboard", patch);
				AddPatchSource(src);
			}
		}

		#endregion

		private void AddPatchSource(IPatchSource patchSource)
		{
			var item = new PatchSourceListItem(patchSource);
			item.CheckedState = Framework.Controls.CheckedState.Checked;
			_lstPatches.Items.Add(item);
			item.FocusAndSelect();
		}

		public bool Execute()
		{
			var applyTo = ApplyTo;
			var reverse = Reverse;
			var sources = SelectedPatchSources;
			try
			{
				Cursor = Cursors.WaitCursor;
				Repository.Status.ApplyPatches(sources, applyTo, reverse);
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToApplyPatch,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}
