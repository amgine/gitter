#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;

using gitter.Framework;

using Resources = gitter.GitLab.Properties.Resources;

public partial class ProviderSetupControl : DialogBase, IExecutableDialog
{
	private readonly IRepository _repository;

	public ProviderSetupControl(HttpMessageInvoker httpMessageInvoker, IRepository repository)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(repository);

		HttpMessageInvoker = httpMessageInvoker;

		InitializeComponent();

		GitterApplication.FontManager.InputFont.Apply(_txtAPIKey);

		_repository = repository;

		Text = Resources.StrGitLab;
		_lblServiceUri.Text = Resources.StrServiceUri.AddColon();
		_lblAPIKey.Text     = Resources.StrAPIKey.AddColon();
		_lblProject.Text    = Resources.StrProject.AddColon();

		var section = _repository.ConfigSection.TryGetSection("IssueTrackers");
		if(section is not null)
		{
			section = section.TryGetSection("GitLab");
		}
		if(section is not null)
		{
			_txtServiceUri.Text = section.GetValue<string>("ServiceUri", string.Empty);
			_txtAPIKey.Text     = Unmask(section.GetValue<string>("APIKey", string.Empty));
			_cmbProject.Text    = section.GetValue<string>("ProjectId", string.Empty);
		}

		_txtAPIKey.TextChanged += OnAPIKeyTextChanged;
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(DefaultWidth, 325));

	private HttpMessageInvoker HttpMessageInvoker { get; }

	private async void OnAPIKeyTextChanged(object? sender, EventArgs e)
	{
		if(string.IsNullOrWhiteSpace(ServiceUri)) return;
		if(string.IsNullOrWhiteSpace(APIKey)) return;
		var server = new ServerInfo(string.Empty, new Uri(ServiceUri), APIKey);
		var svc = new GitLabServiceContext(HttpMessageInvoker, server);

		var projects = await svc.GetProjectsAsync();

		_cmbProject.BeginUpdate();
		_cmbProject.Items.Clear();
		int i = 0;
		foreach(var proj in projects)
		{
			_cmbProject.Items.Add(proj);
			if(((Git.Repository)_repository).Remotes.Any(r => r.FetchUrl == proj.SshUrlToRepo || r.FetchUrl == proj.HttpUrlToRepo))
			{
				_cmbProject.Text = proj.Name;
			}
			++i;
		}
		_cmbProject.EndUpdate();
	}

	private static string Mask(string str)
	{
		if(str == string.Empty) return string.Empty;

		return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
	}

	private static string Unmask(string? str)
	{
		if(str is not { Length: not 0 }) return string.Empty;

		return Encoding.UTF8.GetString(Convert.FromBase64String(str));
	}

	public string ServiceUri
	{
		get => _txtServiceUri.Text.Trim();
		set => _txtServiceUri.Text = value;
	}

	public string APIKey
	{
		get => _txtAPIKey.Text.Trim();
		set => _txtAPIKey.Text = value;
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

	private bool ValidateAPIKey(string key)
	{
		if(string.IsNullOrWhiteSpace(key))
		{
			NotificationService.NotifyInputError(
				_txtAPIKey,
				Resources.ErrNoAPIKeySpecified,
				Resources.ErrAPIKeyCannotBeEmpty);
			return false;
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
		var key = APIKey;
		var pid = ProjectId;

		if(!ValidateServiceUri(uri)) return false;
		if(!ValidateAPIKey(key)) return false;
		if(!ValidateProjectId(pid)) return false;

		var section = _repository
			.ConfigSection
			.GetCreateSection("IssueTrackers")
			.GetCreateSection("GitLab");
		section.SetValue<string>("ServiceUri", uri);
		section.SetValue<string>("APIKey", Mask(key));
		section.SetValue<string>("ProjectId", pid);

		return true;
	}
}
