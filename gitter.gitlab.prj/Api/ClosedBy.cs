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
using System.Text.Json.Serialization;

[DataContract]
sealed class ClosedBy : ModifiableObject
{
	static class Names
	{
		public const string State     = @"active";
		public const string WebUrl    = @"web_url";
		public const string AvatarUrl = @"avatar_url";
		public const string Username  = @"username";
		public const string Name      = @"name";
	}

	[DataMember]
	[JsonPropertyName(Names.State)]
	public string State { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public string WebUrl { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.AvatarUrl)]
	public string AvatarUrl { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Username)]
	public string Username { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;
}
