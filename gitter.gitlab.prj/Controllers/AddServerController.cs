#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

using gitter.Framework.Mvc;

using Resources = gitter.GitLab.Properties.Resources;

class AddServerController : ViewControllerBase<IAddServerView>
{
	public AddServerController(HttpMessageInvoker httpMessageInvoker, IList<ServerInfo> servers)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(servers);

		HttpMessageInvoker = httpMessageInvoker;
		Servers            = servers;
	}

	private HttpMessageInvoker HttpMessageInvoker { get; }

	private IList<ServerInfo> Servers { get; }

	private static bool TryGetServerName(IAddServerView view,
		[MaybeNullWhen(returnValue: false)] out string name)
	{
		name = view.ServerName.Value?.Trim();
		if(name is not { Length: not 0 } || string.IsNullOrWhiteSpace(name))
		{
			view.UserInputErrorNotifier.NotifyError(view.ServerName, new UserInputError(
				Resources.ErrNoServerNameSpecified, Resources.ErrServerNameCannotBeEmpty));
			return false;
		}
		return true;
	}

	private bool TryGetServiceUri(IAddServerView view,
		[MaybeNullWhen(returnValue: false)] out Uri serviceUri)
	{
		var url = view.ServiceUrl.Value?.Trim();
		if(string.IsNullOrWhiteSpace(url))
		{
			view.UserInputErrorNotifier.NotifyError(view.ServiceUrl, new UserInputError(
				Resources.ErrNoServiceUriSpecified, Resources.ErrServiceUriCannotBeEmpty));
			serviceUri = default;
			return false;
		}
		if(!Uri.TryCreate(url, UriKind.Absolute, out serviceUri) || !(serviceUri.Scheme is "http" or "https"))
		{
			view.UserInputErrorNotifier.NotifyError(view.ServiceUrl, new UserInputError(
				Resources.ErrInvalidServiceUri, Resources.ErrServiceUriIsNotValid));
			return false;
		}
		foreach(var existing in Servers)
		{
			if(Equals(existing.ServiceUri, serviceUri))
			{
				view.UserInputErrorNotifier.NotifyError(view.ServiceUrl, new UserInputError(
					Resources.ErrDuplicateServiceUri, Resources.ErrSpecifiedServiceUriAlreadyExists));
				return false;
			}
		}
		return true;
	}

	private static bool TryGetApiKey(IAddServerView view,
		[MaybeNullWhen(returnValue: false)] out string apiKey)
	{
		apiKey = view.APIKey.Value?.Trim();
		if(apiKey is not { Length: not 0 } || string.IsNullOrWhiteSpace(apiKey))
		{
			view.UserInputErrorNotifier.NotifyError(view.APIKey, new UserInputError(
				Resources.ErrNoAPIKeySpecified, Resources.ErrAPIKeyCannotBeEmpty));
			return false;
		}
		return true;
	}

	private bool TryCollectUserInput(IAddServerView view,
		[MaybeNullWhen(returnValue: false)] out ServerInfo server)
	{
		if(!TryGetServerName(view, out var name))       goto fail;
		if(!TryGetServiceUri(view, out var serviceUri)) goto fail;
		if(!TryGetApiKey    (view, out var apiKey))     goto fail;

		server = new ServerInfo(
			name:       name,
			serviceUri: serviceUri,
			apiKey:     apiKey);
		return true;

	fail:
		server = default;
		return false;
	}

	private async Task<bool> ValidateAsync(IAddServerView view, ServerInfo server)
	{
		Assert.IsNotNull(server);

		var api = new Api.ApiEndpoint(HttpMessageInvoker, server);
		try
		{
			using(view.ChangeCursor(MouseCursor.WaitCursor))
			{
				_ = await api.GetVersionAsync();
			}
		}
		catch(UnauthorizedAccessException)
		{
			view.UserInputErrorNotifier.NotifyError(view.APIKey, new UserInputError(
				Resources.ErrApiKeyIsInvalid, Resources.ErrUnableToAuthorize));
			return false;
		}
		catch
		{
			view.UserInputErrorNotifier.NotifyError(view.ServiceUrl, new UserInputError(
				Resources.ErrInvalidServiceUri, Resources.ErrServiceUriIsNotGitLab));
			return false;
		}
		return true;
	}

	public async Task<bool> TryAddServerAsync()
	{
		var view = RequireView();

		if(!TryCollectUserInput(view, out var server)) return false;
		if(!await ValidateAsync(view, server)) return false;

		Servers.Add(server);

		return true;
	}
}
