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
	using System.Collections.Generic;
	using System.ComponentModel;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class ResolveCheckoutDialog : GitDialogBase
	{
		public ResolveCheckoutDialog()
		{
			InitializeComponent();

			Text = Resources.StrCheckout;
		}

		public override DialogButtons OptimalButtons => DialogButtons.Cancel;

		public void SetAvailableBranches(IEnumerable<Branch> branches)
		{
			Verify.Argument.IsNotNull(branches, nameof(branches));
			Verify.Argument.HasNoNullItems(branches, nameof(branches));

			_references.BeginUpdate();
			_references.Style = GitterApplication.DefaultStyle;
			_references.Items.Clear();
			Branch first = null;
			foreach(var branch in branches)
			{
				if(first == null) first = branch;
				_references.Items.Add(new BranchListItem(branch));
			}
			if(first != null)
			{
				SelectedBranch = first;
				UpdateButton();
			}
			_references.EndUpdate();
			if(_references.Items.Count <= 1)
			{
				_references.Visible = false;
				_lblSelectOther.Visible = false;
				Height -= _references.Height + _lblSelectOther.Height;
			}
		}

		public bool CheckoutCommit { get; private set; } = true;

		public Branch SelectedBranch { get; private set; }

		private void UpdateButton()
		{
			_btnCheckoutBranch.Text = string.Format("{0} '{1}'", Resources.StrCheckout, SelectedBranch.Name);
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var b = ((BranchListItem)e.Item).DataContext;
			SelectedBranch = b;
			UpdateButton();
		}

		private void _btnCheckoutCommit_Click(object sender, EventArgs e)
		{
			CheckoutCommit = true;
			GlobalBehavior.AskOnCommitCheckouts = !_chkDontShowAgain.Checked;
			ClickOk();
		}

		private void _btnCheckoutBranch_Click(object sender, EventArgs e)
		{
			CheckoutCommit = false;
			GlobalBehavior.AskOnCommitCheckouts = !_chkDontShowAgain.Checked;
			ClickOk();
		}
	}
}
