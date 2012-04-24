namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class AddParameterDialog : GitDialogBase, IExecutableDialog
	{
		#region Data

		private readonly IWorkingEnvironment _environment;
		private Repository _repository;
		private ConfigFile _configFile;

		#endregion

		/// <summary>Create <see cref="AddParameterDialog"/>.</summary>
		/// <param name="environment">Application environment.</param>
		private AddParameterDialog(IWorkingEnvironment environment)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_environment = environment;

			InitializeComponent();

			Text = Resources.StrAddParameter;

			_lblName.Text = Resources.StrName.AddColon();
			_lblValue.Text = Resources.StrValue.AddColon();

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtValue);
		}

		/// <summary>Create <see cref="AddParameterDialog"/>.</summary>
		/// <param name="environment">Application environment.</param>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		public AddParameterDialog(IWorkingEnvironment environment, Repository repository)
			: this(environment)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			_repository = repository;
			_configFile = ConfigFile.Repository;
		}

		/// <summary>Create <see cref="AddParameterDialog"/>.</summary>
		/// <param name="environment">Application environment.</param>
		/// <param name="configFile">Config file to use.</param>
		public AddParameterDialog(IWorkingEnvironment environment, ConfigFile configFile)
			: this(environment)
		{
			if(configFile != ConfigFile.System && configFile != ConfigFile.User)
			{
				throw new ArgumentNullException("configFile");
			}

			_configFile = configFile;
		}

		protected override string ActionVerb
		{
			get { return Resources.StrAdd; }
		}

		/// <summary>Parameter name.</summary>
		public string ParameterName
		{
			get { return _txtName.Text; }
			set { _txtName.Text = value; }
		}

		/// <summary>Parameter value.</summary>
		public string ParameterValue
		{
			get { return _txtValue.Text; }
			set { _txtValue.Text = value; }
		}

		#region IExecutableDialog Members

		public bool Execute()
		{
			string name = _txtName.Text.Trim();
			string value = _txtValue.Text.Trim();
			if(name.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtName,
					Resources.ErrInvalidParameterName,
					Resources.ErrParameterNameCannotBeEmpty);
				return false;
			}
			try
			{
				Cursor = Cursors.WaitCursor;
				if(_configFile == ConfigFile.Repository)
				{
					var p = _repository.Configuration.TryGetParameter(name);
					if(p != null)
					{
						p.Value = value;
					}
					else
					{
						_repository.Configuration.CreateParameter(name, value);
					}
				}
				else
				{
					RepositoryProvider.Git.SetConfigValue(
						new SetConfigValueParameters(name, value)
						{
							ConfigFile = _configFile,
						});
				}
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToSetParameter,
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		#endregion
	}
}
