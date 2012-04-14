namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Dialog for renaming local branch.</summary>
	[ToolboxItem(false)]
	public partial class RenameBranchDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Branch _branch;

		/// <summary>Create <see cref="RenameBranchDialog"/>.</summary>
		/// <param name="branch">Branch to rename.</param>
		/// <exception cref="ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
		public RenameBranchDialog(Branch branch)
		{
			if(branch == null) throw new ArgumentNullException("branch");
			if(branch.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "branch"), "branch");
			_branch = branch;

			InitializeComponent();

			SetupReferenceNameInputBox(_txtNewName, ReferenceType.LocalBranch);

			Text = Resources.StrRenameBranch;

			_lblOldName.Text = Resources.StrBranch.AddColon();
			_lblNewName.Text = Resources.StrNewName.AddColon();

			_txtOldName.Text = branch.Name;
			_txtNewName.Text = branch.Name;
			_txtNewName.SelectAll();

			GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);
		}

		/// <summary>Verb, describing operation.</summary>
		protected override string ActionVerb
		{
			get { return Resources.StrRename; }
		}

		/// <summary>Branch to rename.</summary>
		public Branch Branch
		{
			get { return _branch; }
		}

		/// <summary>New branche's name.</summary>
		public string NewName
		{
			get { return _txtNewName.Text; }
			set { _txtNewName.Text = value; }
		}

		/// <summary>Perform rename.</summary>
		/// <returns>true if rename succeeded.</returns>
		public bool Execute()
		{
			var repository = _branch.Repository;
			var oldName = _txtOldName.Text;
			var newName = _txtNewName.Text.Trim();

			if(oldName == newName) return true;
			if(!ValidateNewBranchName(newName, _txtNewName, repository))
				return false;

			try
			{
				Cursor = Cursors.WaitCursor;
				_branch.Name = newName;
				Cursor = Cursors.Default;
			}
			catch(BranchAlreadyExistsException)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidBranchName,
					Resources.ErrBranchAlreadyExists);
				return false;
			}
			catch(InvalidBranchNameException exc)
			{
				Cursor = Cursors.Default;
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidBranchName,
					exc.Message);
				return false;
			}
			catch(Exception exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToRenameBranch, oldName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}
