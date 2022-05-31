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

namespace gitter.GitLab.Api;

using System;
using System.Runtime.Serialization;
#if SYSTEM_TEXT_JSON
using System.Text.Json.Serialization;
#elif NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

[DataContract]
class GitLabVersion
{
	static class Names
	{
		public const string Version  = @"version";
		public const string Revision = @"revision";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Version)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Version)]
#endif
	public string Version { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Revision)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Revision)]
#endif
	public string Revision { get; set; }
}
