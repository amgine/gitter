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
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class CleanDialog : DialogBase, IExecutableDialog
	{
		#region Data

		private readonly Repository _repository;
		private FilesToCleanBinding _dataBinding;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="CleanDialog"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		public CleanDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Localize();

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

		#endregion

		private void Localize()
		{
			Text = Resources.StrClean;

			_lblIncludePattern.Text = Resources.StrsIncludePattern.AddColon();
			_lblExcludePattern.Text = Resources.StrsExcludePattern.AddColon();

			_lblType.Text = Resources.StrType.AddColon();

			_radIncludeUntracked.Text = Resources.StrUntracked;
			_radIncludeIgnored.Text = Resources.StrIgnored;
			_radIncludeBoth.Text = Resources.StrBoth;

			_chkRemoveDirectories.Text = Resources.StrsAlsoRemoveDirectories;

			_lblObjectList.Text = Resources.StrsObjectsThatWillBeRemoved.AddColon();
			_lstFilesToClear.Text = Resources.StrsNoFilesToRemove;
		}

		private FilesToCleanBinding DataBinding
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
					if(_dataBinding != null)
					{
						_dataBinding.ReloadData();
					}
				}
			}
		}

		public Repository Repository
		{
			get { return _repository; }
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

		public string IncludePattern
		{
			get { return _txtPattern.Text.Trim(); }
			set { _txtPattern.Text = value; }
		}

		public string ExcludePattern
		{
			get
			{
				if(_txtExclude.Enabled)
				{
					return _txtExclude.Text.Trim();
				}
				else
				{
					return string.Empty;
				}
			}
			set
			{
				Verify.State.IsTrue(_txtExclude.Enabled, "Excluide pattern is not supported.");

				_txtExclude.Text = value;
			}
		}

		public bool RemoveDirectories
		{
			get { return _chkRemoveDirectories.Checked; }
			set { _chkRemoveDirectories.Checked = value; }
		}

		private void UpdateList()
		{
			if(IsDisposed)
			{
				return;
			}

			if(DataBinding == null)
			{
				DataBinding = new FilesToCleanBinding(Repository, _lstFilesToClear);
			}
			DataBinding.IncludePattern = IncludePattern;
			DataBinding.ExcludePattern = ExcludePattern;
			DataBinding.CleanFilesMode = Mode;
			DataBinding.IncludeDirectories = RemoveDirectories;
			DataBinding.ReloadData();
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

		private void OnRemoveDirectoriesCheckedChanged(object sender, EventArgs e)
		{
			UpdateList();
		}

		private void OnFilesToClearItemActivated(object sender, ItemEventArgs e)
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
			try
			{
				Repository.Status.Clean(IncludePattern, ExcludePattern, Mode, RemoveDirectories);
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
