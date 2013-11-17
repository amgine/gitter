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
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class StageDialog : DialogBase, IExecutableDialog
	{
		#region Data

		private readonly Repository _repository;
		private FilesToAddBinding _dataBinding;

		#endregion

		#region .ctor

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

		#endregion

		#region Properties

		public Repository Repository
		{
			get { return _repository; }
		}

		private FilesToAddBinding DataBinding
		{
			get { return _dataBinding; }
			set
			{
				if(_dataBinding != value)
				{
					if(_dataBinding != null)
					{
						_dataBinding.Dispose();
					}
					_dataBinding = value;
				}
			}
		}

		protected override string ActionVerb
		{
			get { return Resources.StrStage; }
		}

		public string Pattern
		{
			get { return _txtPattern.Text.Trim(); }
			set { _txtPattern.Text = value; }
		}

		public bool IncludeUntracked
		{
			get { return _chkIncludeUntracked.Checked; }
			set { _chkIncludeUntracked.Checked = value; }
		}

		public bool IncludeIgnored
		{
			get { return _chkIncludeIgnored.Checked; }
			set { _chkIncludeIgnored.Checked = value; }
		}

		#endregion

		#region Methods

		protected override void OnShown()
		{
			base.OnShown();
			UpdateList();
		}

		private void UpdateList()
		{
			if(DataBinding == null)
			{
				DataBinding = new FilesToAddBinding(Repository, _lstUnstaged);
			}
			DataBinding.Pattern          = Pattern;
			DataBinding.IncludeUntracked = IncludeUntracked;
			DataBinding.IncludeIgnored   = IncludeIgnored;
			DataBinding.ReloadData();
		}

		private void OnPatternTextChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void OnIncludeUntrackedCheckedChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void OnIncludeIgnoredCheckedChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void OnFilesItemActivated(object sender, Framework.Controls.ItemEventArgs e)
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
				Repository.Status.Stage(pattern, addUntracked, addIgnored);
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

		#endregion
	}
}
