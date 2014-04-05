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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Mvc;
	using gitter.Framework.Mvc.WinForms;

	using gitter.Git.Gui.Controllers;
	using gitter.Git.Gui.Controls;
	using gitter.Git.Gui.Interfaces;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class CheckoutDialog : GitDialogBase, IExecutableDialog, ICheckoutView
	{
		#region Data

		private readonly Repository _repository;
		private readonly IUserInputSource<string> _revisionInput;
		private readonly IUserInputErrorNotifier _errorNotifier;
		private readonly ICheckoutController _controller;

		#endregion

		#region .ctor

		public CheckoutDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			var inputs = new IUserInputSource[]
			{
				_revisionInput = new TextBoxInputSource(_txtRevision),
			};
			_errorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

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

		#endregion

		#region Properties

		protected override string ActionVerb
		{
			get { return Resources.StrCheckout; }
		}

		public IUserInputSource<string> Revision
		{
			get { return _revisionInput; }
		}

		public IUserInputErrorNotifier ErrorNotifier
		{
			get { return _errorNotifier; }
		}

		#endregion

		#region Methods

		private void OnReferencesItemActivated(object sender, ItemEventArgs e)
		{
			if((e.Item is BranchListItem) || (e.Item is TagListItem))
			{
				ClickOk();
			}
		}

		private bool TrySetRevisionText(BranchListItem listItem)
		{
			if(listItem != null)
			{
				var branch = listItem.DataContext;
				_txtRevision.Text = branch.Name;
				return true;
			}
			return false;
		}

		private bool TrySetRevisionText(RemoteBranchListItem listItem)
		{
			if(listItem != null)
			{
				var branch = listItem.DataContext;
				_txtRevision.Text = branch.Name;
				return true;
			}
			return false;
		}

		private bool TrySetRevisionText(TagListItem listItem)
		{
			if(listItem != null)
			{
				var tag = listItem.DataContext;
				_txtRevision.Text = tag.Name;
				return true;
			}
			return false;
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			if(_lstReferences.SelectedItems.Count != 1) return;
			var item = _lstReferences.SelectedItems[0];

			if(TrySetRevisionText(item as BranchListItem)) return;
			if(TrySetRevisionText(item as RemoteBranchListItem)) return;
			if(TrySetRevisionText(item as TagListItem)) return;
		}

		#endregion

		#region IExecutableDialog

		public bool Execute()
		{
			return _controller.TryCheckout();
		}

		#endregion
	}
}
