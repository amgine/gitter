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

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class CheckoutDialog : GitDialogBase, IExecutableDialog, ICheckoutView
{
	readonly struct DialogControls
	{
		public readonly ReferencesListBox _lstReferences;
		public readonly TextBox _txtRevision;
		public readonly LabelControl _lblRevision;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_lstReferences = new();
			_txtRevision = new();
			_lblRevision = new();

			_lstReferences.DisableContextMenus = true;
			_lstReferences.HeaderStyle         = HeaderStyle.Hidden;
			_lstReferences.ShowTreeLines       = true;
		}

		public void Localize()
		{
			_lblRevision.Text = Resources.StrRevision.AddColon();
		}

		public void Layout(Control parent)
		{
			var revisionDec = new TextBoxDecorator(_txtRevision);
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
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblRevision,   marginOverride: LayoutConstants.NoMargin), row: 0, column: 0),
						new GridContent(new ControlContent(revisionDec,    marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lstReferences, marginOverride: LayoutConstants.NoMargin), row: 1, columnSpan: 2),
					]),
			};

			var tabIndex = 0;
			_lblRevision.TabIndex = tabIndex++;
			revisionDec.TabIndex = tabIndex++;
			_lstReferences.TabIndex = tabIndex++;

			_lblRevision.Parent = parent;
			revisionDec.Parent = parent;
			_lstReferences.Parent = parent;
		}
	}

	private readonly Repository _repository;
	private readonly DialogControls _controls;
	private readonly ICheckoutController _controller;

	public CheckoutDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;

		Name = nameof(CheckoutDialog);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Size = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			Revision = new TextBoxInputSource(_controls._txtRevision),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		Text = Resources.StrCheckoutRevision;

		_controls._lstReferences.LoadData(_repository, ReferenceType.Reference, GlobalBehavior.GroupReferences, GlobalBehavior.GroupRemoteBranches);
		_controls._lstReferences.Items[0].IsExpanded = true;
		_controls._lstReferences.ItemActivated += OnReferencesItemActivated;
		_controls._lstReferences.SelectionChanged += OnSelectionChanged;

		GlobalBehavior.SetupAutoCompleteSource(_controls._txtRevision, _repository, ReferenceType.Reference);
		GitterApplication.FontManager.InputFont.Apply(_controls._txtRevision);

		_controller = new CheckoutController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 325));

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCheckout;

	public IUserInputSource<string?> Revision { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		_controls._txtRevision.SelectAll();
		BeginInvoke(_controls._txtRevision.Focus);
	}

	private void OnReferencesItemActivated(object? sender, ItemEventArgs e)
	{
		if(e.Item is BranchListItem or TagListItem)
		{
			ClickOk();
		}
	}

	private bool TrySetRevisionText(IDataContextProvider<IRevisionPointer>? listItem)
	{
		if(listItem is null) return false;

		var @object = listItem.DataContext;
		_controls._txtRevision.Text = @object.Pointer;
		return true;
	}

	private void OnSelectionChanged(object? sender, EventArgs e)
	{
		if(_controls._lstReferences.SelectedItems.Count != 1) return;
		var item = _controls._lstReferences.SelectedItems[0];

		TrySetRevisionText(item as IDataContextProvider<IRevisionPointer>);
	}

	/// <inheritdoc/>
	public bool Execute() => _controller.TryCheckout();
}
