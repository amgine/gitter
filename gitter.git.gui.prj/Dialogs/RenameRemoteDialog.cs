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
	public partial class RenameRemoteDialog : GitDialogBase, IExecutableDialog, IRenameRemoteView
	{
		#region Data

		private readonly IRenameRemoteController _controller;

		#endregion

		#region .ctor

		public RenameRemoteDialog(Remote remote)
		{
			Verify.Argument.IsNotNull(remote, nameof(remote));
			Verify.Argument.IsFalse(remote.IsDeleted, nameof(remote),
				Resources.ExcObjectIsDeleted.UseAsFormat(nameof(Remote)));

			Remote = remote;

			InitializeComponent();
			Localize();

			SetupReferenceNameInputBox(_txtNewName, ReferenceType.Remote);

			_txtOldName.Text = remote.Name;
			_txtNewName.Text = remote.Name;
			_txtNewName.SelectAll();

			var inputs = new IUserInputSource[]
			{
				NewName = new TextBoxInputSource(_txtNewName),
			};
			ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

			GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);

			_controller = new RenameRemoteController(remote) { View = this };
		}

		#endregion

		#region Properties

		protected override string ActionVerb => Resources.StrRename;

		public Remote Remote { get; }

		public IUserInputSource<string> NewName { get; }

		public IUserInputErrorNotifier ErrorNotifier { get; }

		#endregion

		#region Methods

		private void Localize()
		{
			Text = Resources.StrRenameRemote;

			_lblOldName.Text = Resources.StrRemote.AddColon();
			_lblNewName.Text = Resources.StrNewName.AddColon();
		}

		#endregion

		#region IExecutableDialog Members

		public bool Execute() => _controller.TryRename();

		#endregion
	}
}
