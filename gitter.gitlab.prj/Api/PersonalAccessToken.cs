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
class PersonalAccessToken
{
	static class Names
	{
		public const string Id          = @"id";
		public const string Name        = @"name";
		public const string Revoked     = @"revoked";
		public const string CreatedAt   = @"created_at";
		public const string Description = @"description";
		public const string Scopes      = @"scopes";
		public const string UserId      = @"user_id";
		public const string LastUsedAt  = @"last_used_at";
		public const string Active      = @"active";
		public const string ExpiresAt   = @"expires_at";
	}

	[DataMember]
	[JsonPropertyName(Names.Id)]
	public long Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Revoked)]
	public bool Revoked { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CreatedAt)]
	public DateTimeOffset CreatedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Description)]
	public string? Description { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Scopes)]
	public string[] Scopes { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.UserId)]
	public long UserId { get; set; }

	[DataMember]
	[JsonPropertyName(Names.LastUsedAt)]
	public DateTimeOffset? LastUsedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Active)]
	public bool Active { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ExpiresAt)]
	public DateTimeOffset? ExpiresAt { get; set; }
}

[DataContract]
class PersonalAccessTokenWithSecret : PersonalAccessToken
{
	static class Names
	{
		public const string Token = @"token";
	}

	[DataMember]
	[JsonPropertyName(Names.Token)]
	public string Token { get; set; } = default!;
}
