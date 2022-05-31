#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Redmine.Gui;

using System;
using System.Drawing;

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

		var section = _repository.ConfigSection.TryGetSection("IssueTrackers")?.TryGetSection("Redmine");
		if(section is not null)
		{
			_txtServiceUri.Text = section.GetValue<string>("ServiceUri", string.Empty);
			_txtApiKey.Text     = section.GetValue<string>("ApiKey", string.Empty);
			_cmbProject.Text    = section.GetValue<string>("ProjectId", string.Empty);
		}
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 87));

	public string ServiceUri
	{
		get => _txtServiceUri.Text.Trim();
		set => _txtServiceUri.Text = value;
	}

	public string ApiKey
	{
		get => _txtApiKey.Text.Trim();
		set => _txtApiKey.Text = value;
	}

	public string ProjectId
	{
		get => _cmbProject.Text.Trim();
		set => _cmbProject.Text = value;
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
		if(!Uri.TryCreate(uri, UriKind.Absolute, out _))
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
			if(!Uri.IsHexDigit(apiKey[i]))
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
