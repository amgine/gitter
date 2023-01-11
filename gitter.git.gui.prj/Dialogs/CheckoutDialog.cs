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
using gitter.Framework.Controls;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

#nullable enable

[ToolboxItem(false)]
public partial class CheckoutDialog : GitDialogBase, IExecutableDialog, ICheckoutView
{
	private readonly Repository _repository;
	private readonly ICheckoutController _controller;

	public CheckoutDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;

		InitializeComponent();

		var inputs = new IUserInputSource[]
		{
			Revision = new TextBoxInputSource(_txtRevision),
		};
		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		Text = Resources.StrCheckoutRevision;

		_lblRevision.Text = Resources.StrRevision.AddColon();

		_lstReferences.Style = GitterApplication.DefaultStyle;
		_lstReferences.LoadData(_repository, ReferenceType.Reference, GlobalBehavior.GroupReferences, GlobalBehavior.GroupRemoteBranches);
		_lstReferences.Items[0].IsExpanded = true;
		_lstReferences.ItemActivated += OnReferencesItemActivated;

		GlobalBehavior.SetupAutoCompleteSource(_txtRevision, _repository, ReferenceType.Reference);
		GitterApplication.FontManager.InputFont.Apply(_txtRevision);

		_controller = new CheckoutController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 325));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCheckout;

	public IUserInputSource<string> Revision { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		_txtRevision.SelectAll();
		BeginInvoke(_txtRevision.Focus);
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
		if(listItem is not null)
		{
			var @object = listItem.DataContext;
			_txtRevision.Text = @object.Pointer;
			return true;
		}
		return false;
	}

	private void OnSelectionChanged(object? sender, EventArgs e)
	{
		if(_lstReferences.SelectedItems.Count != 1) return;
		var item = _lstReferences.SelectedItems[0];

		TrySetRevisionText(item as IDataContextProvider<IRevisionPointer>);
	}

	/// <inheritdoc/>
	public bool Execute() => _controller.TryCheckout();
}
