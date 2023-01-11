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

/// <summary>Dialog for creating <see cref="Tag"/> object.</summary>
[ToolboxItem(false)]
public partial class CreateTagDialog : GitDialogBase, IExecutableDialog, ICreateTagView
{
	private Repository _repository;
	private TextBoxSpellChecker _speller;
	private readonly ICreateTagController _controller;

	/// <summary>Create <see cref="CreateTagDialog"/>.</summary>
	/// <param name="repository"><see cref="Repository"/> to create <see cref="Tag"/> in.</param>
	/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
	public CreateTagDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;

		InitializeComponent();
		Localize();

		var inputs = new IUserInputSource[]
		{
			TagName   = new TextBoxInputSource(_txtName),
			Revision  = new ControlInputSource(_txtRevision),
			Message   = new TextBoxInputSource(_txtMessage),
			Annotated = new RadioButtonInputSource(_radAnnotated),
			Signed    = new RadioButtonInputSource(_radSigned),
			UseKeyId  = new RadioButtonInputSource(_radUseKeyId),
			KeyId     = new TextBoxInputSource(_txtKeyId),
		};

		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_txtName, ReferenceType.Tag);

		_txtRevision.References.LoadData(
			_repository,
			ReferenceType.Reference,
			GlobalBehavior.GroupReferences,
			GlobalBehavior.GroupRemoteBranches);
		_txtRevision.References.Items[0].IsExpanded = true;

		GitterApplication.FontManager.InputFont.Apply(_txtKeyId, _txtMessage, _txtName, _txtRevision);
		GlobalBehavior.SetupAutoCompleteSource(_txtRevision, _repository, ReferenceType.Branch);
		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_txtMessage, true);
		}

		_controller = new CreateTagController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 364));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCreate;

	public IUserInputSource<string> TagName { get; }

	public IUserInputSource<string> Revision { get; }

	public IUserInputSource<string> Message { get; }

	public IUserInputSource<bool> Signed { get; }

	public IUserInputSource<bool> Annotated { get; }

	public IUserInputSource<bool> UseKeyId { get; }

	public IUserInputSource<string> KeyId { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		BeginInvoke(_txtName.Focus);
		base.OnLoad(e);
	}

	private void Localize()
	{
		Text				= Resources.StrCreateTag;

		_lblName.Text		= Resources.StrName.AddColon();
		_lblRevision.Text	= Resources.StrRevision.AddColon();

		_grpOptions.Text	= Resources.StrType;
		_radSimple.Text		= Resources.StrSimpleTag;
		_radAnnotated.Text	= Resources.StrAnnotatedTag;
		_radSigned.Text		= Resources.StrSigned;
			
		_grpMessage.Text	= Resources.StrMessage;
			
		_grpSigning.Text	= Resources.StrSigning;
		_radUseDefaultEmailKey.Text = Resources.StrUseDefaultEmailKey;
		_radUseKeyId.Text	= Resources.StrUseKeyId.AddColon();
	}

	private void SetControlStates()
	{
		bool signed = _radSigned.Checked;
		bool annotated = signed || _radAnnotated.Checked;
		_txtMessage.Enabled = annotated;
		_radUseDefaultEmailKey.Enabled = signed;
		_radUseKeyId.Enabled = signed;
		_txtKeyId.Enabled = signed & _radUseKeyId.Checked;
	}

	private void _radSimple_CheckedChanged(object sender, EventArgs e)
	{
		SetControlStates();
	}

	private void _radAnnotated_CheckedChanged(object sender, EventArgs e)
	{
		SetControlStates();
	}

	private void _radSigned_CheckedChanged(object sender, EventArgs e)
	{
		SetControlStates();
	}

	private void _radUseKeyId_CheckedChanged(object sender, EventArgs e)
	{
		_txtKeyId.Enabled = _radUseKeyId.Checked;
	}

	/// <summary>Create <see cref="Tag"/>.</summary>
	/// <returns>true, if <see cref="Tag"/> was created successfully.</returns>
	public bool Execute() => _controller.TryCreateTag();
}
