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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;
using gitter.Framework.Services;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for creating <see cref="Note"/> object.</summary>
[ToolboxItem(false)]
public partial class AddNoteDialog : GitDialogBase, IExecutableDialog, IAddNoteView
{
	private TextBoxSpellChecker _speller;
	private readonly IAddNoteController _controller;

	/// <summary>Create <see cref="AddNoteDialog"/>.</summary>
	/// <param name="repository">Repository to create note in.</param>
	public AddNoteDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		InitializeComponent();

		var inputs = new IUserInputSource[]
		{
			Revision = new ControlInputSource(_txtRevision),
			Message  = new TextBoxInputSource(_txtMessage),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		Text = Resources.StrAddNote;

		_txtRevision.References.LoadData(
			Repository,
			ReferenceType.Reference,
			GlobalBehavior.GroupReferences,
			GlobalBehavior.GroupRemoteBranches);
		_txtRevision.References.Items[0].IsExpanded = true;

		_txtRevision.Text = GitConstants.HEAD;

		_lblRevision.Text = Resources.StrRevision.AddColon();
		_lblMessage.Text = Resources.StrMessage.AddColon();

		GitterApplication.FontManager.InputFont.Apply(_txtRevision, _txtMessage);
		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_txtMessage, true);
		}
		_controller = new AddNoteController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 241));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrAdd;

	public Repository Repository { get; }

	public IUserInputSource<string> Revision { get; }

	public IUserInputSource<string> Message { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_txtMessage.Focus);
	}

	/// <inheritdoc/>
	public bool Execute() => _controller.TryAddNote();
}
