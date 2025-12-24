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

namespace gitter.GitLab;

using System;

using gitter.Framework.Configuration;

public sealed class ServerInfo(
	string name,
	Uri    serviceUri,
	string apiKey)
{
	public string Name { get; set; } = name;

	public Uri ServiceUri { get; set; } = serviceUri;

	public string ApiKey { get; set; } = apiKey;

	internal static ServerInfo LoadFrom(Section section)
	{
		Verify.Argument.IsNotNull(section);

		return new ServerInfo(
			name:       section.GetValue<string>("Name") ?? "",
			serviceUri: new Uri(section.GetValue<string>("ServiceUrl") ?? ""),
			apiKey:     section.GetValue<string>("ApiKey") ?? "");
	}

	internal void SaveTo(Section section)
	{
		Verify.Argument.IsNotNull(section);

		section.SetValue("Name",       Name);
		section.SetValue("ServiceUrl", ServiceUri.ToString());
		section.SetValue("ApiKey",     ApiKey);
	}
}
