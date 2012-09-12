namespace gitter.Git.Gui.Dialogs
{
	using System;

	using gitter.Framework;

	public partial class BranchPropertiesDialog : GitDialogBase
	{
		private Branch _branch;

		public BranchPropertiesDialog(Branch branch)
		{
			Verify.Argument.IsNotNull(branch, "branch");

			_branch = branch;

			InitializeComponent();
		}

		public Branch Branch
		{
			get { return _branch; }
		}
	}
}
