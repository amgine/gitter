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
	[JsonPropertyName(Names.Id)]
	public long Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.NameWithNamespace)]
	public string NameWithNamespace { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Path)]
	public string Path { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.PathWithNamespace)]
	public string PathWithNamespace { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.SshUrlToRepo)]
	public string? SshUrlToRepo { get; set; }

	[DataMember]
	[JsonPropertyName(Names.HttpUrlToRepo)]
	public string? HttpUrlToRepo { get; set; }

	public override string ToString() => NameWithNamespace;
}
