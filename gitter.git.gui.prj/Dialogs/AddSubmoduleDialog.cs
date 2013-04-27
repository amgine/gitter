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
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class AddSubmoduleDialog : GitDialogBase, IExecutableDialog
	{
		private Repository _repository;

		/// <summary>Create <see cref="AddSubmoduleDialog"/>.</summary>
		public AddSubmoduleDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrAddSubmodule;

			_lblPath.Text = Resources.StrPath.AddColon();
			_lblUrl.Text = Resources.StrUrl.AddColon();
			_chkBranch.Text = Resources.StrBranch.AddColon();

			GitterApplication.FontManager.InputFont.Apply(_txtBranch, _txtRepository, _txtPath);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public string Path
		{
			get { return _txtRepository.Text; }
			set { _txtRepository.Text = value; }
		}

		public string Repository
		{
			get { return _txtPath.Text; }
			set { _txtPath.Text = value; }
		}

		public string Branch
		{
			get { return _txtBranch.Text; }
			set { _txtBranch.Text = value; }
		}

		public bool UseCustomBranch
		{
			get { return _chkBranch.Checked; }
			set { _chkBranch.Checked = value; }
		}

		private void _chkBranch_CheckedChanged(object sender, EventArgs e)
		{
			_txtBranch.Enabled = _chkBranch.Checked;
		}

		#region IExecutableDialog

		public bool Execute()
		{
			var path = _txtPath.Text.Trim();
			if(!ValidateRelativePath(path, _txtPath))
			{
				return false;
			}
			var url = _txtRepository.Text.Trim();
			if(!ValidateUrl(url, _txtRepository))
			{
				return false;
			}
			var branch = _chkBranch.Checked ? _txtBranch.Text.Trim() : null;
			if(!string.IsNullOrWhiteSpace(branch))
			{
				if(!ValidateBranchName(branch, _txtBranch))
				{
					return false;
				}
			}
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					_repository.Submodules.Create(path, url, branch);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToAddSubmodule, path),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
