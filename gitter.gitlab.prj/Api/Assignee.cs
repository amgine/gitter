#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
public sealed class Assignee
{
	static class Names
	{
		public const string Id        = @"id";
		public const string State     = @"state";
		public const string AvatarUrl = @"avatar_url";
		public const string Name      = @"name";
		public const string CreatedAt = @"created_at";
		public const string Username  = @"username";
		public const string WebUrl    = @"web_url";
	}

	[DataMember]
	[JsonPropertyName(Names.Id)]
	public int Id { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.State)]
	public string State { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.AvatarUrl)]
	public string AvatarUrl { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.CreatedAt)]
	public DateTime CreatedAt { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Username)]
	public string Username { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public string WebUrl { get; set; } = default!;
}
