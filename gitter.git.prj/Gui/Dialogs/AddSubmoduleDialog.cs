namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class AddSubmoduleDialog : GitDialogBase, IExecutableDialog
	{
		private Repository _repository;

		/// <summary>Create <see cref="AddSubmoduleDialog"/>.</summary>
		public AddSubmoduleDialog(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;

			InitializeComponent();

			Text = Resources.StrAddSubmodule;

			_lblPath.Text = Resources.StrPath.AddColon();
			_lblUrl.Text = Resources.StrUrl.AddColon();
			_chkBranch.Text = Resources.StrBranch.AddColon();

			GitterApplication.FontManager.InputFont.Apply(_txtBranch, _txtRepository, _txtPath);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public string Path
		{
			get { return _txtRepository.Text; }
			set { _txtRepository.Text = value; }
		}

		public string Repository
		{
			get { return _txtPath.Text; }
			set { _txtPath.Text = value; }
		}

		public string Branch
		{
			get { return _txtBranch.Text; }
			set { _txtBranch.Text = value; }
		}

		public bool UseCustomBranch
		{
			get { return _chkBranch.Checked; }
			set { _chkBranch.Checked = value; }
		}

		private void _chkBranch_CheckedChanged(object sender, EventArgs e)
		{
			_txtBranch.Enabled = _chkBranch.Checked;
		}

		public bool Execute()
		{
			var path = _txtPath.Text.Trim();
			var url = _txtRepository.Text.Trim();
			var branch = _chkBranch.Checked ? _txtBranch.Text.Trim() : null;
			try
			{
				Cursor = Cursors.WaitCursor;
				_repository.Submodules.Create(path, url, branch);
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToAddSubmodule, path),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}
