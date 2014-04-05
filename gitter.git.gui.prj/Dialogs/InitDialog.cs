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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;

	using gitter.Framework;
	using gitter.Framework.Mvc;
	using gitter.Framework.Mvc.WinForms;

	using gitter.Git.Gui.Controllers;
	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class InitDialog : GitDialogBase, IExecutableDialog, IInitView
	{
		#region Data

		private readonly IGitRepositoryProvider _gitRepositoryProvider;
		private readonly IUserInputSource<string> _repositoryPathInput;
		private readonly IUserInputSource<bool> _bareInput;
		private readonly IUserInputSource<bool> _useCustomTemplateInput;
		private readonly IUserInputSource<string> _templateInput;
		private readonly IUserInputErrorNotifier _errorNotifier;
		private readonly IInitController _controller;

		#endregion

		#region .ctor

		public InitDialog(IGitRepositoryProvider gitRepositoryProvider)
		{
			Verify.Argument.IsNotNull(gitRepositoryProvider, "gitRepositoryProvider");

			_gitRepositoryProvider = gitRepositoryProvider;

			InitializeComponent();
			Localize();

			var inputs = new IUserInputSource[]
			{
				_repositoryPathInput    = new TextBoxInputSource(_txtPath),
				_bareInput              = new CheckBoxInputSource(_chkBare),
				_useCustomTemplateInput = new CheckBoxInputSource(_chkUseTemplate),
				_templateInput          = new TextBoxInputSource(_txtTemplate),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

			GitterApplication.FontManager.InputFont.Apply(_txtPath, _txtTemplate);

			_controller = new InitController(gitRepositoryProvider) { View = this };
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrInit; }
		}

		private IGitRepositoryProvider GitRepositoryProvider
		{
			get { return _gitRepositoryProvider; }
		}

		public IUserInputSource<string> RepositoryPath
		{
			get { return _repositoryPathInput; }
		}

		public IUserInputSource<bool> Bare
		{
			get { return _bareInput; }
		}

		public IUserInputSource<bool> UseCustomTemplate
		{
			get { return _useCustomTemplateInput; }
		}

		public IUserInputSource<string> Template
		{
			get { return _templateInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region Methods

		private void Localize()
		{
			Text = Resources.StrInitRepository;

			_lblPath.Text        = Resources.StrPath.AddColon();
			_grpOptions.Text     = Resources.StrOptions;
			_chkUseTemplate.Text = Resources.StrTemplate.AddColon();
			_chkBare.Text        = Resources.StrBare;
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

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			return _controller.TryInit();
		}

		#endregion
	}
}
