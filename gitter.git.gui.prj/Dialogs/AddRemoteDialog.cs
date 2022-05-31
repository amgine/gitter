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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class AddRemoteDialog : GitDialogBase, IAsyncExecutableDialog, IAddRemoteView
{
	private readonly IAddRemoteController _controller;

	public AddRemoteDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		InitializeComponent();
		Localize();

		var inputs = new IUserInputSource[]
		{
			RemoteName   = new TextBoxInputSource(_txtName),
			Url          = new TextBoxInputSource(_txtUrl),
			Fetch        = new CheckBoxInputSource(_chkFetch),
			Mirror       = new CheckBoxInputSource(_chkMirror),
			TagFetchMode = new RadioButtonGroupInputSource<TagFetchMode>(
				new[]
				{
					Tuple.Create(_tagFetchDefault, gitter.Git.TagFetchMode.Default),
					Tuple.Create(_tagFetchNone,    gitter.Git.TagFetchMode.NoTags),
					Tuple.Create(_tagFetchAll,     gitter.Git.TagFetchMode.AllTags),
				}),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_txtName, ReferenceType.Remote);

		if(Repository.Remotes.Count == 0)
		{
			_txtName.Text = GitConstants.DefaultRemoteName;
		}

		GitterApplication.FontManager.InputFont.Apply(_txtName, _txtUrl);

		_controller = new AddRemoteController(repository) { View = this };
	}

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

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 156));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrAdd;

	public Repository Repository { get; }

	public IUserInputSource<string> RemoteName { get; }

	public IUserInputSource<string> Url { get; }

	public IUserInputSource<bool> Fetch { get; }

	public IUserInputSource<bool> Mirror { get; }

	public IUserInputSource<TagFetchMode> TagFetchMode { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	public Task<bool> ExecuteAsync() => _controller.TryAddRemoteAsync();
}
