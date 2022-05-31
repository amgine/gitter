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
class Project
{
	static class Names
	{
		public const string Id                = @"id";
		public const string Name              = @"name";
		public const string NameWithNamespace = @"name_with_namespace";
		public const string Path              = @"path";
		public const string PathWithNamespace = @"path_with_namespace";
		public const string SshUrlToRepo      = @"ssh_url_to_repo";
		public const string HttpUrlToRepo     = @"http_url_to_repo";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Id)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Id)]
#endif
	public long Id { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Name)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Name)]
#endif
	public string Name { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.NameWithNamespace)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.NameWithNamespace)]
#endif
	public string NameWithNamespace { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Path)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Path)]
#endif
	public string Path { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.PathWithNamespace)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.PathWithNamespace)]
#endif
	public string PathWithNamespace { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.SshUrlToRepo)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.SshUrlToRepo)]
#endif
	public string SshUrlToRepo { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.HttpUrlToRepo)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.HttpUrlToRepo)]
#endif
	public string HttpUrlToRepo { get; set; }

	public override string ToString() => NameWithNamespace;
}
