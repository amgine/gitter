namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Controls;
	using gitter.Framework.Options;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class CheckoutDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Repository _repository;

		public CheckoutDialog(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			InitializeComponent();

			Text = Resources.StrCheckoutRevision;

			_lblRevision.Text = Resources.StrRevision.AddColon();

			_lstReferences.Style = GitterApplication.DefaultStyle;
			_lstReferences.LoadData(_repository, ReferenceType.Reference, GlobalBehavior.GroupReferences, GlobalBehavior.GroupRemoteBranches);
			_lstReferences.Items[0].IsExpanded = true;
			_lstReferences.ItemActivated += OnReferencesItemActivated;

			GlobalBehavior.SetupAutoCompleteSource(_txtRevision, _repository, ReferenceType.Reference);
			GitterApplication.FontManager.InputFont.Apply(_txtRevision);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrCheckout; }
		}

		public string Revision
		{
			get { return _txtRevision.Text; }
			set { _txtRevision.Text = value; }
		}

		private void OnReferencesItemActivated(object sender, ItemEventArgs e)
		{
			if(e.Item is BranchListItem)
			{
				ClickOk();
			}
			else if(e.Item is TagListItem)
			{
				ClickOk();
			}
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			if(_lstReferences.SelectedItems.Count != 1) return;
			var item = _lstReferences.SelectedItems[0];
			if(item is BranchListItem)
			{
				var branch = ((BranchListItem)item).DataContext;
				_txtRevision.Text = branch.Name;
			}
			else if(item is RemoteBranchListItem)
			{
				var branch = ((RemoteBranchListItem)item).DataContext;
				_txtRevision.Text = branch.Name;
			}
			else if(item is TagListItem)
			{
				var tag = ((TagListItem)item).DataContext;
				_txtRevision.Text = tag.Name;
			}
		}

		private void ProceedCheckout(IRevisionPointer revision)
		{
			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					revision.Checkout(true);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToCheckout, revision.Pointer),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
			}
		}

		#region IExecutableDialog

		public bool Execute()
		{
			var revision = _txtRevision.Text.Trim();
			if(string.IsNullOrWhiteSpace(revision)) return true;

			var pointer = _repository.GetRevisionPointer(revision);
			bool force = Control.ModifierKeys == Keys.Shift;

			try
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					pointer.Checkout(force);
				}
			}
			catch(UnknownRevisionException)
			{
				NotificationService.NotifyInputError(
					_txtRevision,
					Resources.ErrInvalidRevisionExpression,
					Resources.ErrRevisionIsUnknown);
				return false;
			}
			catch(UntrackedFileWouldBeOverwrittenException)
			{
				if(GitterApplication.MessageBoxService.Show(
					this,
					string.Format(Resources.AskOverwriteUntracked, revision),
					Resources.StrCheckout,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					ProceedCheckout(pointer);
				}
			}
			catch(HaveLocalChangesException)
			{
				if(GitterApplication.MessageBoxService.Show(
					this,
					string.Format(Resources.AskThrowAwayLocalChanges, revision),
					Resources.StrCheckout,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					ProceedCheckout(pointer);
				}
			}
			catch(HaveConflictsException)
			{
				if(GitterApplication.MessageBoxService.Show(
					this,
					string.Format(Resources.AskThrowAwayConflictedChanges, revision),
					Resources.StrCheckout,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					ProceedCheckout(pointer);
				}
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToCheckout, _txtRevision.Text),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
