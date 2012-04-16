namespace gitter
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	internal partial class AddRepositoryDialog : DialogBase, IExecutableDialog
	{
		private readonly IWorkingEnvironment _environment;
		private readonly LocalRepositoriesListBox _repositoryList;

		public AddRepositoryDialog(IWorkingEnvironment environment, LocalRepositoriesListBox repositoriyList)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			if(repositoriyList == null) throw new ArgumentNullException("repositoriesList");
			_environment = environment;
			_repositoryList = repositoriyList;

			InitializeComponent();

			Text = Resources.StrAddRepository;

			_lblPath.Text = Resources.StrPath.AddColon();
			_lblDescription.Text = Resources.StrDescription.AddColon();

			GitterApplication.FontManager.InputFont.Apply(_txtPath, _txtDescription);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		public string Path
		{
			get { return _txtPath.Text; }
			set { _txtPath.Text = value; }
		}

		public bool AllowChangePath
		{
			get { return !_txtPath.ReadOnly; }
			set { _txtPath.ReadOnly = !value; }
		}

		public string Description
		{
			get { return _txtDescription.Text; }
			set { _txtDescription.Text = value; }
		}

		private void _btnSelectDirectory_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtPath.Text = path;
			}
		}

		public bool Execute()
		{
			var path = _txtPath.Text.Trim();
			if(path.Length == 0)
			{
				NotificationService.NotifyInputError(_txtPath,
					Resources.ErrInvalidPath,
					Resources.ErrPathCannotBeEmpty);
				return false;
			}
			var prov = _environment.FindProviderForDirectory(path);
			if(prov == null)
			{
				NotificationService.NotifyInputError(_txtPath,
					Resources.ErrInvalidPath,
					Resources.ErrPathIsNotValidRepository.UseAsFormat(path));
				return false;
			}
			var item = new RepositoryListItem(new RepositoryLink(path, prov.Name) { Description = _txtDescription.Text });
			_repositoryList.Items.Add(item);
			return true;
		}
	}
}
