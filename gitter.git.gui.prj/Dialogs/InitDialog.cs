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
	using System.IO;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class InitDialog : GitDialogBase, IExecutableDialog
	{
		private readonly IGitRepositoryProvider _gitRepositoryProvider;
		private string _repositoryPath;

		public InitDialog(IGitRepositoryProvider gitRepositoryProvider)
		{
			Verify.Argument.IsNotNull(gitRepositoryProvider, "gitRepositoryProvider");

			InitializeComponent();

			Text = Resources.StrInitRepository;

			_gitRepositoryProvider = gitRepositoryProvider;

			_lblPath.Text = Resources.StrPath.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_chkUseTemplate.Text = Resources.StrTemplate.AddColon();
			_chkBare.Text = Resources.StrBare;

			GitterApplication.FontManager.InputFont.Apply(_txtPath, _txtTemplate);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if(!string.IsNullOrWhiteSpace(RepositoryPath))
			{
				_txtPath.Text = RepositoryPath.Trim();
			}
		}

		public bool AllowChangeRepositoryPath
		{
			get { return !_txtPath.ReadOnly; }
			set { _txtPath.ReadOnly = !value; }
		}

		public string RepositoryPath
		{
			get { return _repositoryPath; }
			set { _repositoryPath = value; }
		}

		public string TemplatePath
		{
			get { return _txtTemplate.Text; }
			set { _txtTemplate.Text = value; }
		}

		public bool UseTemplate
		{
			get { return _chkUseTemplate.Checked; }
			set { _chkUseTemplate.Checked = value; }
		}

		public bool Bare
		{
			get { return _chkBare.Checked; }
			set { _chkBare.Checked = value; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrInit; }
		}

		private IGitRepositoryProvider GitRepositoryProvider
		{
			get { return _gitRepositoryProvider; }
		}

		private void _btnSelectDirectory_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtPath.Text = path;
			}
		}

		private void _btnSelectTemplate_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtTemplate.Text = path;
			}
		}

		private void _chkUseTemplate_CheckedChanged(object sender, EventArgs e)
		{
			bool check = _chkUseTemplate.Checked;
			_txtTemplate.Enabled = check;
			_btnSelectTemplate.Enabled = check;
		}

		public bool Execute()
		{
			_repositoryPath = _txtPath.Text.Trim();
			if(!ValidateAbsolutePath(_repositoryPath, _txtPath))
			{
				return false;
			}
			string template = null;
			if(_chkUseTemplate.Checked)
			{
				template = _txtTemplate.Text.Trim();
				if(!ValidateAbsolutePath(_repositoryPath, _txtTemplate))
				{
					return false;
				}
			}
			bool bare = _chkBare.Checked;
			try
			{
				if(!Directory.Exists(_repositoryPath))
				{
					Directory.CreateDirectory(_repositoryPath);
				}
			}
			catch(Exception exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToCreateDirectory,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Repository.Init(GitRepositoryProvider.GitAccessor, _repositoryPath, template, bare);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToInit,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}
