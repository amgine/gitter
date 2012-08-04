namespace gitter.Git
{
	using System;
	using System.Linq;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Options;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class ConfigurationPage : PropertyPage
	{
		public static readonly new Guid Guid = new Guid("AE583E68-3D3E-4A89-AE92-7A89527EDAA3");

		private readonly IWorkingEnvironment _environment;
		private readonly ConfigurationFile _systemCfg;
		private readonly ConfigurationFile _userCfg;

		public ConfigurationPage(IWorkingEnvironment environment)
			: base(Guid)
		{
			if(environment == null) throw new ArgumentNullException("environment");

			_environment = environment;

			InitializeComponent();

			_pageUser.Text = Resources.StrCurrentUser;
			_pageSystem.Text = Resources.StrSystem;

			_btnAddUserParameter.Text = Resources.StrAddParameter;
			_btnAddSystemParameter.Text = Resources.StrAddParameter;

			var gitAccessor = environment.RepositoryProviders
										 .OfType<IGitRepositoryProvider>()
										 .First()
										 .GitAccessor;

			_userCfg	= ConfigurationFile.OpenCurrentUserFile(gitAccessor);
			_systemCfg	= ConfigurationFile.OpenSystemFile(gitAccessor);
			_lstUserConfig.LoadData(_userCfg);
			_lstSystemConfig.LoadData(_systemCfg);
		}

		private void _addUserParameter_Click(object sender, EventArgs e)
		{
			using(var dlg = new AddParameterDialog(_environment, ConfigFile.User))
			{
				if(dlg.Run(this) == DialogResult.OK)
				{
					_userCfg.Refresh();
				}
			}
		}

		private void _addSystemParameter_Click(object sender, EventArgs e)
		{
			using(var dlg = new AddParameterDialog(_environment, ConfigFile.System))
			{
				if(dlg.Run(this) == DialogResult.OK)
				{
					_systemCfg.Refresh();
				}
			}
		}
	}
}
