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

	public partial class ProviderSetupControl : DialogBase, IExecutableDialog
	{
		private IRepository _repository;

		public ProviderSetupControl(IRepository repository)
		{
			InitializeComponent();

			GitterApplication.FontManager.InputFont.Apply(_txtApiKey);

			_repository = repository;

			Text = Resources.StrRedmine;
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

		public string ServiceUri
		{
			get { return _txtServiceUri.Text.Trim(); }
			set { _txtServiceUri.Text = value; }
		}

		public string ApiKey
		{
			get { return _txtApiKey.Text.Trim(); }
			set { _txtApiKey.Text = value; }
		}

		public string ProjectId
		{
			get { return _cmbProject.Text.Trim(); }
			set { _cmbProject.Text = value; }
		}

		private bool ValidateServiceUri(string uri)
		{
			if(string.IsNullOrWhiteSpace(uri))
			{
				NotificationService.NotifyInputError(
					_txtServiceUri,
					Resources.ErrNoServiceUriSpecified,
					Resources.ErrServiceUriCannotBeEmpty);
				return false;
			}
			Uri dummy;
			if(!Uri.TryCreate(uri, UriKind.Absolute, out dummy))
			{
				NotificationService.NotifyInputError(
					_txtServiceUri,
					Resources.ErrInvalidServiceUri,
					Resources.ErrServiceUriIsNotValid);
			}
			return true;
		}

		private bool ValidateApiKey(string apiKey)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
			{
				NotificationService.NotifyInputError(
					_txtApiKey,
					Resources.ErrNoApiKeySpecified,
					Resources.ErrApiKeyCannotBeEmpty);
				return false;
			}
			if(apiKey.Length != 40)
			{
				NotificationService.NotifyInputError(
					_txtApiKey,
					Resources.ErrInvalidApiKey,
					Resources.ErrApiKeyMustContain40Characters);
				return false;
			}
			for(int i = 0; i < apiKey.Length; ++i)
			{
				if(!apiKey[i].IsHexDigit())
				{
					NotificationService.NotifyInputError(
						_txtApiKey,
						Resources.ErrInvalidApiKey,
						Resources.ErrApiKeyContainsInvalidCharacters);
					return false;
				}
			}
			return true;
		}

		private bool ValidateProjectId(string projectId)
		{
			if(string.IsNullOrWhiteSpace(projectId))
			{
				NotificationService.NotifyInputError(
					_cmbProject,
					Resources.ErrNoProjectNameSpecified,
					Resources.ErrProjectNameCannotBeEmpty);
				return false;
			}
			return true;
		}

		public bool Execute()
		{
			var uri = ServiceUri;
			var key = ApiKey;
			var pid = ProjectId;

			if(!ValidateServiceUri(uri)) return false;
			if(!ValidateApiKey(key)) return false;
			if(!ValidateProjectId(pid)) return false;

			var section = _repository.ConfigSection
									 .GetCreateSection("IssueTrackers")
									 .GetCreateSection("Redmine");
			section.SetValue<string>("ServiceUri", uri);
			section.SetValue<string>("ApiKey", key);
			section.SetValue<string>("ProjectId", pid);

			return true;
		}
	}
}
