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

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gitter.GitLab.Api;

[DataContract]
sealed class User
{
	static class Names
	{
		public const string Id               = @"id";
		public const string Username         = @"username";
		public const string Name             = @"name";
		public const string CurrentSignInAt  = @"current_sign_in_at";
		public const string Linkedin         = @"linkedin";
		public const string CanCreateProject = @"can_create_project";
		public const string Bio              = @"bio";
		public const string AvatarUrl        = @"avatar_url";
		public const string CanCreateGroup   = @"can_create_group";
		public const string ConfirmedAt      = @"confirmed_at";
		public const string ColorSchemeId    = @"color_scheme_id";
		public const string CreatedAt        = @"created_at";
		public const string Identities       = @"identities";
		public const string External         = @"external";
		public const string Email            = @"email";
		public const string LastActivityOn   = @"last_activity_on";
		public const string IsAdmin          = @"is_admin";
		public const string LastSignInAt     = @"last_sign_in_at";
		public const string ProjectsLimit    = @"projects_limit";
		public const string TwoFactorEnabled = @"two_factor_enabled";
		public const string Location         = @"location";
		public const string Organization     = @"organization";
		public const string State            = @"state";
		public const string Skype            = @"skype";
		public const string Twitter          = @"twitter";
		public const string WebUrl           = @"web_url";
		public const string WebsiteUrl       = @"website_url";
	}

	[DataMember]
	[JsonPropertyName(Names.Id)]
	public long Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Username)]
	public string Username { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string? Name { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CurrentSignInAt)]
	public string? CurrentSignInAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Linkedin)]
	public string? Linkedin { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CanCreateProject)]
	public bool CanCreateProject { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Bio)]
	public string? Bio { get; set; }

	[DataMember]
	[JsonPropertyName(Names.AvatarUrl)]
	public string? AvatarUrl { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CanCreateGroup)]
	public bool CanCreateGroup { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ConfirmedAt)]
	public string? ConfirmedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ColorSchemeId)]
	public int ColorSchemeId { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CreatedAt)]
	public string? CreatedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Identities)]
	public Identity[]? Identities { get; set; }

	[DataMember]
	[JsonPropertyName(Names.External)]
	public bool External { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Email)]
	public string? Email { get; set; }

	[DataMember]
	[JsonPropertyName(Names.LastActivityOn)]
	public string? LastActivityOn { get; set; }

	[DataMember]
	[JsonPropertyName(Names.IsAdmin)]
	public bool IsAdmin { get; set; }

	[DataMember]
	[JsonPropertyName(Names.LastSignInAt)]
	public string? LastSignInAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ProjectsLimit)]
	public int ProjectsLimit { get; set; }

	[DataMember]
	[JsonPropertyName(Names.TwoFactorEnabled)]
	public bool TwoFactorEnabled { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Location)]
	public string? Location { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Organization)]
	public string? Organization { get; set; }

	[DataMember]
	[JsonPropertyName(Names.State)]
	public string? State { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Skype)]
	public string? Skype { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Twitter)]
	public string? Twitter { get; set; }

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public string? WebUrl { get; set; }

	[DataMember]
	[JsonPropertyName(Names.WebsiteUrl)]
	public string? WebsiteUrl { get; set; }
}
