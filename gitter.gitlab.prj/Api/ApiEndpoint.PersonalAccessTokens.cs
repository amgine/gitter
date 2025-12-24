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

namespace gitter.GitLab.Api;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

partial class ApiEndpoint
{
	private static void AppendPersonalAccessTokenSelfUrl(StringBuilder urlBuilder)
	{
		urlBuilder.Append(@"api/v4/personal_access_tokens/self");
	}

	private static void AppendPersonalAccessTokenUrl(StringBuilder urlBuilder, long id)
	{
		urlBuilder.Append(@"api/v4/personal_access_tokens/");
		urlBuilder.Append(id);
	}

	public Task<PersonalAccessToken?> GetPersonalAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendPersonalAccessTokenSelfUrl(query);

		return GetAsync<PersonalAccessToken>(query.ToString(), cancellationToken);
	}

	public Task<PersonalAccessTokenWithSecret?> SelfRotatePersonalAccessTokenAsync(DateTime? expiresAt = default, CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendPersonalAccessTokenSelfUrl(query);
		query.Append("/rotate");
		if(expiresAt.HasValue)
		{
			query.Append("?expires_at=");
			query.Append(expiresAt.Value.ToString("yyyy-MM-dd"));
		}

		return PostAsync<PersonalAccessTokenWithSecret>(query.ToString(), cancellationToken);
	}

	public Task<PersonalAccessToken?> GetPersonalAccessTokenAsync(long id, CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendPersonalAccessTokenUrl(query, id);

		return GetAsync<PersonalAccessToken>(query.ToString(), cancellationToken);
	}
}
