namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.IO;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class InitDialog : GitDialogBase, IExecutableDialog
	{
		public InitDialog()
		{
			InitializeComponent();

			Text = Resources.StrInitRepository;

			_lblPath.Text = Resources.StrPath.AddColon();
			_grpOptions.Text = Resources.StrOptions;
			_chkUseTemplate.Text = Resources.StrTemplate.AddColon();
			_chkBare.Text = Resources.StrBare;

			GitterApplication.FontManager.InputFont.Apply(_txtPath, _txtTemplate);
		}

		public bool AllowChangeRepositoryPath
		{
			get { return !_txtPath.ReadOnly; }
			set { _txtPath.ReadOnly = !value; }
		}

		public string RepositoryPath
		{
			get { return _txtPath.Text; }
			set { _txtPath.Text = value; }
		}

		public string TemplatePath
		{
			get { return _txtTemplate.Text; }
			set { _txtTemplate.Text = value; }
		}

		public bool UseTemplate
		{
			get { return _chkUseTemplate.Checked; }
			set { _chkUseTemplate.Checked = value; }
		}

		public bool Bare
		{
			get { return _chkBare.Checked; }
			set { _chkBare.Checked = value; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrInit; }
		}

		private void _btnSelectDirectory_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtPath.Text = path;
			}
		}

		private void _btnSelectTemplate_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtTemplate.Text = path;
			}
		}

		private void _chkUseTemplate_CheckedChanged(object sender, EventArgs e)
		{
			bool check = _chkUseTemplate.Checked;
			_txtTemplate.Enabled = check;
			_btnSelectTemplate.Enabled = check;
		}

		public bool Execute()
		{
			var path = _txtPath.Text.Trim();
			try
			{
				if(!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}
			catch(Exception exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToCreateDirectory,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			string template = null;
			if(_chkUseTemplate.Checked)
			{
				template = _txtTemplate.Text.Trim();
				if(!ValidatePath(path, _txtTemplate)) return false;
			}
			bool bare = _chkBare.Checked;
			try
			{
				Cursor = Cursors.WaitCursor;
				Repository.Init(path, template, bare);
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToInit,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}
