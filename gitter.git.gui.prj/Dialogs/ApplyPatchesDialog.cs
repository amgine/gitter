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
		#region Data

		private Repository _repository;

		#endregion

		#region .ctor

		public ApplyPatchesDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrApplyPatches;

			_lstPatches.Style = GitterApplication.DefaultStyle;
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
			_radWorkingDirectory.Text = Resources.StrsWorkingDirectory;
			_radIndexOnly.Text = Resources.StrIndex;
			_radIndexAndWorkingDirectory.Text = Resources.StrsIndexAndWorkingDirectory;
			_grpOptions.Text = Resources.StrOptions;
			_chkReverse.Text = Resources.StrReverse;
		}

		#endregion

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

		#region Methods

		private void AddPatchSource(IPatchSource patchSource)
		{
			Assert.IsNotNull(patchSource);

			var item = new PatchSourceListItem(patchSource);
			item.CheckedState = Framework.Controls.CheckedState.Checked;
			_lstPatches.Items.Add(item);
			item.FocusAndSelect();
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
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
			}
			if(!string.IsNullOrWhiteSpace(patch))
			{
				var src = new PatchFromString("Patch from clipboard", patch);
				AddPatchSource(src);
			}
		}

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			var applyTo = ApplyTo;
			var reverse = Reverse;
			var sources = SelectedPatchSources;
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Repository.Status.ApplyPatches(sources, applyTo, reverse);
				}
			}
			catch(GitException exc)
			{
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

		#endregion
	}
}
