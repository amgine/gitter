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

namespace gitter.GitLab;

using gitter.Framework;

static class UrlHelper
{
	static string? GetPersonalAccessTokensUrl(string? url)
	{
		if(url is not { Length: not 0 }) return default;
		if(!url.StartsWith("http://") && !url.StartsWith("https://")) return default;

		return url.EndsWith('/')
			? url + "-/user_settings/personal_access_tokens"
			: url + "/-/user_settings/personal_access_tokens";
	}

	public static bool OpenPersonalAccessTokensPage(string serviceUrl)
	{
		var url = GetPersonalAccessTokensUrl(serviceUrl);
		if(url is null) return false;
		Utility.OpenUrl(url);
		return true;
	}
}
