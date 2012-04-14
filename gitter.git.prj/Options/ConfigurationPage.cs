namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework.Options;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class ConfigurationPage : PropertyPage
	{
		public static readonly new Guid Guid = new Guid("AE583E68-3D3E-4A89-AE92-7A89527EDAA3");

		public ConfigurationPage()
			: base(Guid)
		{
			InitializeComponent();

			_pageUser.Text = Resources.StrCurrentUser;
			_pageSystem.Text = Resources.StrSystem;

			_btnAddUserParameter.Text = Resources.StrAddParameter;
			_btnAddSystemParameter.Text = Resources.StrAddParameter;

			_lstUserConfig.LoadData(ConfigFile.User);
			_lstSystemConfig.LoadData(ConfigFile.System);
		}

		private void _addUserParameter_Click(object sender, EventArgs e)
		{
			using(var dlg = new AddParameterDialog(ConfigFile.User))
			{
				if(dlg.Run(this) == DialogResult.OK)
				{
					_lstUserConfig.LoadData(ConfigFile.User);
				}
			}
		}

		private void _addSystemParameter_Click(object sender, EventArgs e)
		{
			using(var dlg = new AddParameterDialog(ConfigFile.System))
			{
				if(dlg.Run(this) == DialogResult.OK)
				{
					_lstSystemConfig.LoadData(ConfigFile.System);
				}
			}
		}
	}
}
