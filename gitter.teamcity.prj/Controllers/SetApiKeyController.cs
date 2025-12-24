#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.TeamCity;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

using gitter.Framework.Mvc;

using Resources = gitter.TeamCity.Properties.Resources;

class SetApiKeyController : ViewControllerBase<ISetApiKeyView>
{
	public SetApiKeyController(HttpMessageInvoker httpMessageInvoker, IList<ServerInfo> servers)
	{
		Verify.Argument.IsNotNull(httpMessageInvoker);
		Verify.Argument.IsNotNull(servers);

		HttpMessageInvoker = httpMessageInvoker;
		Servers            = servers;
	}

	private HttpMessageInvoker HttpMessageInvoker { get; }

	private IList<ServerInfo> Servers { get; }

	private static bool TryGetApiKey(ISetApiKeyView view, [MaybeNullWhen(returnValue: false)] out string apiKey)
	{
		var value = view.APIKey.Value;
		if(value is not { Length: not 0 } || string.IsNullOrWhiteSpace(value))
		{
			view.UserInputErrorNotifier.NotifyError(view.APIKey, new UserInputError(
				Resources.ErrNoAPIKeySpecified, Resources.ErrAPIKeyCannotBeEmpty));
			apiKey = default;
			return false;
		}
		apiKey = value.Trim();
		return true;
	}

	private async Task<bool> ValidateAsync(ISetApiKeyView view, ServerInfo server)
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
		catch
		{
			view.UserInputErrorNotifier.NotifyError(view.APIKey, new UserInputError(
				Resources.ErrApiKeyIsInvalid, Resources.ErrUnableToAuthorize));
			return false;
		}
		return true;
	}

	private ServerInfo? FindServerInfo(Uri serviceUri, out int index)
	{
		for(int i = 0; i < Servers.Count; ++i)
		{
			if(Servers[i].ServiceUri == serviceUri)
			{
				index = i;
				return Servers[i];
			}
		}
		index = -1;
		return default;
	}

	public async Task<bool> TrySetApiKeyAsync()
	{
		var view = RequireView();

		var uri = view.ServiceUri;
		var server = FindServerInfo(uri, out var index);
		if(server is null) return false;
		if(!TryGetApiKey(view, out var apiKey)) return false;
		if(server.ApiKey == apiKey) return true;
		var temp = new ServerInfo(server.Name, server.ServiceUri, apiKey);
		if(!await ValidateAsync(view, temp)) return false;
		server.ApiKey = apiKey;
		return true;
	}
}
