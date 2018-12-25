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
	public partial class AddSubmoduleDialog : GitDialogBase, IExecutableDialog, IAddSubmoduleView
	{
		#region Data

		private Repository _repository;
		private readonly IAddSubmoduleController _controller;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="AddSubmoduleDialog"/>.</summary>
		public AddSubmoduleDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			_repository = repository;

			InitializeComponent();
			Localize();

			var inputs = new IUserInputSource[]
			{
				Path            = new TextBoxInputSource(_txtPath),
				Url             = new TextBoxInputSource(_txtRepository),
				UseCustomBranch = new CheckBoxInputSource(_chkBranch),
				BranchName      = new TextBoxInputSource(_txtBranch),
			};
			ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

			GitterApplication.FontManager.InputFont.Apply(_txtBranch, _txtRepository, _txtPath);

			_controller = new AddSubmoduleController(repository) { View = this };
		}

		#endregion

		#region Properties

		protected override string ActionVerb => Resources.StrAdd;

		public IUserInputSource<string> Path { get; }

		public IUserInputSource<string> Url { get; }

		public IUserInputSource<bool> UseCustomBranch { get; }

		public IUserInputSource<string> BranchName { get; }

		public IUserInputErrorNotifier ErrorNotifier { get; }

		#endregion

		#region Methods

		private void Localize()
		{
			Text = Resources.StrAddSubmodule;

			_lblPath.Text   = Resources.StrPath.AddColon();
			_lblUrl.Text    = Resources.StrUrl.AddColon();
			_chkBranch.Text = Resources.StrBranch.AddColon();
		}

		private void _chkBranch_CheckedChanged(object sender, EventArgs e)
		{
			_txtBranch.Enabled = _chkBranch.Checked;
		}

		#endregion

		#region IExecutableDialog

		public bool Execute() => _controller.TryAddSubmodule();

		#endregion
	}
}
