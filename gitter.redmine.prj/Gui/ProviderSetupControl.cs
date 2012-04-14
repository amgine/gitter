namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;

	using Resources = gitter.Redmine.Properties.Resources;

	public partial class ProviderSetupControl : UserControl, IExecutableDialog
	{
		private IRepository _repository;

		public ProviderSetupControl(IRepository repository)
		{
			InitializeComponent();

			_repository = repository;

			_lblServiceUri.Text = Resources.StrServiceUri.AddColon();
			_lblApiKey.Text = Resources.StrApiKey.AddColon();
			_lblProject.Text = Resources.StrProject.AddColon();

			var section = _repository.ConfigSection.TryGetSection("IssueTrackers");
			if(section != null)
			{
				section = section.TryGetSection("Redmine");
			}
			if(section != null)
			{
				_txtServiceUri.Text = section.GetValue<string>("ServiceUri", string.Empty);
				_txtApiKey.Text = section.GetValue<string>("ApiKey", string.Empty);
				_cmbProject.Text = section.GetValue<string>("ProjectId", string.Empty);
			}
		}

		public bool Execute()
		{
			var key = _txtApiKey.Text.Trim();
			var uri = _txtServiceUri.Text.Trim();
			var pid = _cmbProject.Text.Trim();

			if(string.IsNullOrEmpty(key)) return false;
			if(string.IsNullOrEmpty(uri)) return false;
			if(string.IsNullOrEmpty(pid)) return false;

			var section = _repository.ConfigSection.GetCreateSection("IssueTrackers").GetCreateSection("Redmine");
			section.SetValue<string>("ServiceUri", uri);
			section.SetValue<string>("ApiKey", key);
			section.SetValue<string>("ProjectId", pid);

			return true;
		}
	}
}
