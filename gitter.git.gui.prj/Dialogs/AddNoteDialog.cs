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
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;
using gitter.Framework.Services;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for creating <see cref="Note"/> object.</summary>
[ToolboxItem(false)]
public partial class AddNoteDialog : GitDialogBase, IExecutableDialog, IAddNoteView
{
	readonly struct DialogControls
	{
		public readonly TextBox _txtMessage;
		public readonly LabelControl _lblMessage;
		public readonly RevisionPicker _txtRevision;
		public readonly LabelControl _lblRevision;

		public DialogControls(IGitterStyle? style)
		{
			style ??= GitterApplication.Style;

			_txtMessage = new()
			{
				AcceptsReturn = true,
				AcceptsTab = true,
				Multiline = true,
				ScrollBars = ScrollBars.None,
			};
			_lblMessage  = new();
			_txtRevision = new();
			_lblRevision = new();

			GitterApplication.FontManager.InputFont.Apply(_txtRevision, _txtMessage);
		}

		public void Localize()
		{
			_lblRevision.Text = Resources.StrRevision.AddColon();
			_lblMessage.Text  = Resources.StrMessage.AddColon();
		}

		public void Layout(Control parent)
		{
			TextBoxDecorator messageDec;

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.LabelRowHeight,
						LayoutConstants.LabelRowSpacing,
						SizeSpec.Everything(),
					],
					columns:
					[
						SizeSpec.Absolute(100),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblRevision, marginOverride: LayoutConstants.TextBoxLabelMargin), row: 0, column: 0),
						new GridContent(new ControlContent(_txtRevision, marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblMessage, marginOverride: LayoutConstants.NoMargin), row: 1, columnSpan: 2),
						new GridContent(new ControlContent(messageDec = new(_txtMessage), marginOverride: LayoutConstants.NoMargin), row: 3, columnSpan: 2),
					]),
			};

			_lblRevision.Parent = parent;
			_txtRevision.Parent = parent;
			_lblMessage.Parent  = parent;
			messageDec.Parent  = parent;

			var tabIndex = 0;
			_lblRevision.TabIndex = tabIndex++;
			_txtRevision.TabIndex = tabIndex++;
			_lblMessage.TabIndex  = tabIndex++;
			messageDec.TabIndex  = tabIndex++;
		}
	}

	private readonly DialogControls _controls;
	private readonly IAddNoteController _controller;
	private TextBoxSpellChecker? _speller;

	/// <summary>Create <see cref="AddNoteDialog"/>.</summary>
	/// <param name="repository">Repository to create note in.</param>
	public AddNoteDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		Repository = repository;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size = ScalableSize.GetValue(Dpi.Default);
		Name = nameof(AddNoteDialog);
		Text = Resources.StrAddNote;
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			Revision = new ControlInputSource(_controls._txtRevision),
			Message  = new TextBoxInputSource(_controls._txtMessage),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		Text = Resources.StrAddNote;

		_controls._txtRevision.References.LoadData(
			Repository,
			ReferenceType.Reference,
			GlobalBehavior.GroupReferences,
			GlobalBehavior.GroupRemoteBranches);
		_controls._txtRevision.References.Items[0].IsExpanded = true;

		_controls._txtRevision.Text = GitConstants.HEAD;

		_controls._lblRevision.Text = Resources.StrRevision.AddColon();
		_controls._lblMessage.Text = Resources.StrMessage.AddColon();

		if(SpellingService.Enabled)
		{
			_speller = new TextBoxSpellChecker(_controls._txtMessage, true);
		}
		_controller = new AddNoteController(repository) { View = this };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			_speller?.Dispose();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 241));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrAdd;

	public Repository Repository { get; }

	public IUserInputSource<string> Revision { get; }

	public IUserInputSource<string?> Message { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtMessage.Focus);
	}

	/// <inheritdoc/>
	public bool Execute() => _controller.TryAddNote();
}
