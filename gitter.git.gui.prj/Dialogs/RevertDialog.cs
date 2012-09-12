namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class RevertDialog : GitDialogBase, IExecutableDialog
	{
		private IRevisionPointer _revision;

		public RevertDialog(IRevisionPointer revision)
		{
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsFalse(revision.IsDeleted, "revision",
				Resources.ExcObjectIsDeleted.UseAsFormat(revision.GetType().Name));

			_revision = revision;

			InitializeComponent();

			Text = Resources.StrRevertCommit;

			_lblRevision.Text = Resources.StrRevision.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_chkNoCommit.Text = Resources.StrNoCommit;

			_txtRevision.Text = _revision.Pointer;

			GitterApplication.FontManager.InputFont.Apply(_txtRevision);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrRevert; }
		}

		public IRevisionPointer Revision
		{
			get { return _revision; }
		}

		public bool Execute()
		{
			bool noCommit = _chkNoCommit.Checked;
			try
			{
				_revision.Revert(noCommit);
			}
			catch(Exception exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToRevert,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}
