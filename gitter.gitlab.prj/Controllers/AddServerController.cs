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

namespace gitter.GitLab
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using gitter.Framework.Mvc;

	using Resources = gitter.GitLab.Properties.Resources;

	class AddServerController : ViewControllerBase<IAddServerView>
	{
		public AddServerController(IList<ServerInfo> servers)
		{
			Verify.Argument.IsNotNull(servers, nameof(servers));

			Servers = servers;
		}

		private IList<ServerInfo> Servers { get; }

		public async Task<bool> TryAddServerAsync()
		{
			Verify.State.IsTrue(View != null, "Controller is not attached to a view.");

			var name = View.ServerName.Value.Trim();
			if(string.IsNullOrWhiteSpace(name))
			{
				View.UserInputErrorNotifier.NotifyError(View.ServerName, new UserInputError(
					Resources.ErrNoServerNameSpecified, Resources.ErrServerNameCannotBeEmpty));
				return false;
			}
			var url = View.ServiceUrl.Value.Trim();
			if(string.IsNullOrWhiteSpace(url))
			{
				View.UserInputErrorNotifier.NotifyError(View.ServiceUrl, new UserInputError(
					Resources.ErrNoServiceUriSpecified, Resources.ErrServiceUriCannotBeEmpty));
				return false;
			}
			if(!Uri.TryCreate(url, UriKind.Absolute, out var serviceUri) || (serviceUri.Scheme != "http" && serviceUri.Scheme != "https"))
			{
				View.UserInputErrorNotifier.NotifyError(View.ServiceUrl, new UserInputError(
					Resources.ErrInvalidServiceUri, Resources.ErrServiceUriIsNotValid));
				return false;
			}
			foreach(var existing in Servers)
			{
				if(Equals(existing.ServiceUrl, serviceUri))
				{
					View.UserInputErrorNotifier.NotifyError(View.ServiceUrl, new UserInputError(
						Resources.ErrDuplicateServiceUri, Resources.ErrSpecifiedServiceUriAlreadyExists));
					return false;
				}
			}
			var key = View.APIKey.Value.Trim();
			if(string.IsNullOrWhiteSpace(key))
			{
				View.UserInputErrorNotifier.NotifyError(View.APIKey, new UserInputError(
					Resources.ErrNoAPIKeySpecified, Resources.ErrAPIKeyCannotBeEmpty));
				return false;
			}

			var api = new Api.ApiEndpoint(serviceUri, key);
			try
			{
				await api.GetVersionAsync();
			}
			catch(UnauthorizedAccessException)
			{
				View.UserInputErrorNotifier.NotifyError(View.APIKey, new UserInputError(
					Resources.ErrApiKeyIsInvalid, Resources.ErrUnableToAuthorize));
				return false;
			}
			catch
			{
				View.UserInputErrorNotifier.NotifyError(View.ServiceUrl, new UserInputError(
					Resources.ErrInvalidServiceUri, Resources.ErrServiceUriIsNotGitLab));
				return false;
			}

			var server = new ServerInfo
			{
				Name       = name,
				ServiceUrl = serviceUri,
				ApiKey     = key,
			};

			Servers.Add(server);

			return true;
		}
	}
}
