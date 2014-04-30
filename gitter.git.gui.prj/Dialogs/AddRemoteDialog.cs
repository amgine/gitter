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
	public partial class AddRemoteDialog : GitDialogBase, IExecutableDialog, IAddRemoteView
	{
		#region Data

		private readonly Repository _repository;
		private readonly IAddRemoteController _controller;
		private readonly IUserInputSource<string> _remoteNameInput;
		private readonly IUserInputSource<string> _urlInput;
		private readonly IUserInputSource<bool> _fetchInput;
		private readonly IUserInputSource<bool> _mirrorInput;
		private readonly IUserInputSource<TagFetchMode> _tagFetchModeInput;
		private readonly IUserInputErrorNotifier _errorNotifier;

		#endregion

		#region .ctor

		public AddRemoteDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();
			Localize();

			var inputs = new IUserInputSource[]
			{
				_remoteNameInput   = new TextBoxInputSource(_txtName),
				_urlInput          = new TextBoxInputSource(_txtUrl),
				_fetchInput        = new CheckBoxInputSource(_chkFetch),
				_mirrorInput       = new CheckBoxInputSource(_chkMirror),
				_tagFetchModeInput = new RadioButtonGroupInputSource<TagFetchMode>(
					new[]
					{
						Tuple.Create(_tagFetchDefault, gitter.Git.TagFetchMode.Default),
						Tuple.Create(_tagFetchNone,    gitter.Git.TagFetchMode.NoTags),
						Tuple.Create(_tagFetchAll,     gitter.Git.TagFetchMode.AllTags),
					}),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

			SetupReferenceNameInputBox(_txtName, ReferenceType.Remote);

			if(_repository.Remotes.Count == 0)
			{
				_txtName.Text = GitConstants.DefaultRemoteName;
			}

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtUrl);

			_controller = new AddRemoteController(repository) { View = this };
		}

		#endregion

		#region Methods

		private void Localize()
		{
			Text = Resources.StrAddRemote;

			_lblName.Text          = Resources.StrName.AddColon();
			_lblUrl.Text           = Resources.StrUrl.AddColon();
			
			_grpOptions.Text       = Resources.StrOptions;
			_chkFetch.Text         = Resources.StrsFetchRemote;
			_chkMirror.Text        = Resources.StrMirror;

			_grpBranches.Text      = Resources.StrTrackingBranches;
			_trackAllBranches.Text = Resources.StrlTrackAllBranches;
			_trackSpecified.Text   = Resources.StrlTrackSpecifiedBranches.AddColon();

			_grpTagImport.Text     = Resources.StrsTagFetchMode;
			_tagFetchDefault.Text  = Resources.StrDefault;
			_tagFetchAll.Text      = Resources.StrsFetchAll;
			_tagFetchNone.Text     = Resources.StrsFetchNone;
		}

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public Repository Repository
		{
			get { return _repository; }
		}

		public IUserInputSource<string> RemoteName
		{
			get { return _remoteNameInput; }
		}

		public IUserInputSource<string> Url
		{
			get { return _urlInput; }
		}

		public IUserInputSource<bool> Fetch
		{
			get { return _fetchInput; }
		}

		public IUserInputSource<bool> Mirror
		{
			get { return _mirrorInput; }
		}

		public IUserInputSource<TagFetchMode> TagFetchMode
		{
			get { return _tagFetchModeInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			return _controller.TryAddRemote();
		}

		#endregion
	}
}
