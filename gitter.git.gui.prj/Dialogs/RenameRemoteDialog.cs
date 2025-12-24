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
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class RenameRemoteDialog : GitDialogBase, IExecutableDialog, IRenameRemoteView
{
	readonly struct DialogControls
	{
		public readonly TextBox _txtOldName;
		public readonly TextBox _txtNewName;
		public readonly LabelControl _lblOldName;
		public readonly LabelControl _lblNewName;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			_txtOldName = new() { ReadOnly = true };
			_txtNewName = new();
			_lblOldName = new();
			_lblNewName = new();

			GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);
		}

		public void Localize()
		{
			_lblOldName.Text = Resources.StrRemote.AddColon();
			_lblNewName.Text = Resources.StrNewName.AddColon();
		}

		public void Layout(Control parent)
		{
			var oldNameDec = new TextBoxDecorator(_txtOldName);
			var newNameDec = new TextBoxDecorator(_txtNewName);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblOldName, marginOverride: LayoutConstants.NoMargin), column: 0, row: 0),
						new GridContent(new ControlContent(oldNameDec, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 0),
						new GridContent(new ControlContent(_lblNewName, marginOverride: LayoutConstants.NoMargin), column: 0, row: 1),
						new GridContent(new ControlContent(newNameDec, marginOverride: LayoutConstants.TextBoxMargin), column: 1, row: 1),
					]),
			};

			var tabIndex = 0;
			_lblOldName.TabIndex = tabIndex++;
			oldNameDec.TabIndex = tabIndex++;
			_lblNewName.TabIndex = tabIndex++;
			newNameDec.TabIndex = tabIndex++;

			_lblOldName.Parent = parent;
			oldNameDec.Parent = parent;
			_lblNewName.Parent = parent;
			newNameDec.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private readonly IRenameRemoteController _controller;

	public RenameRemoteDialog(Remote remote)
	{
		Verify.Argument.IsNotNull(remote);
		Verify.Argument.IsFalse(remote.IsDeleted, nameof(remote),
			Resources.ExcObjectIsDeleted.UseAsFormat(nameof(Remote)));

		Remote = remote;

		Name = nameof(RenameRemoteDialog);
		Text = Resources.StrRenameRemote;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		SetupReferenceNameInputBox(_controls._txtNewName, ReferenceType.Remote);

		_controls._txtOldName.Text = remote.Name;
		_controls._txtNewName.Text = remote.Name;
		_controls._txtNewName.SelectAll();

		var inputs = new IUserInputSource[]
		{
			NewName = new TextBoxInputSource(_controls._txtNewName),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		_controller = new RenameRemoteController(remote) { View = this };
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 53));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrRename;

	public Remote Remote { get; }

	public IUserInputSource<string?> NewName { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtNewName.Focus);
	}

	/// <inheritdoc/>
	public bool Execute() => _controller.TryRename();
}
