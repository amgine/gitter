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
#if SYSTEM_TEXT_JSON
using System.Text.Json.Serialization;
#elif NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

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
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Id)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Id)]
#endif
	public long Id { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Username)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Username)]
#endif
	public string Username { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Name)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Name)]
#endif
	public string Name { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CurrentSignInAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CurrentSignInAt)]
#endif
	public string CurrentSignInAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Linkedin)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Linkedin)]
#endif
	public string Linkedin { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CanCreateProject)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CanCreateProject)]
#endif
	public bool CanCreateProject { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Bio)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Bio)]
#endif
	public string Bio { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.AvatarUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.AvatarUrl)]
#endif
	public string AvatarUrl { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CanCreateGroup)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CanCreateGroup)]
#endif
	public bool CanCreateGroup { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ConfirmedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ConfirmedAt)]
#endif
	public string ConfirmedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ColorSchemeId)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ColorSchemeId)]
#endif
	public int ColorSchemeId { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CreatedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CreatedAt)]
#endif
	public string CreatedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Identities)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Identities)]
#endif
	public Identity[] Identities { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.External)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.External)]
#endif
	public bool External { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Email)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Email)]
#endif
	public string Email { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.LastActivityOn)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.LastActivityOn)]
#endif
	public string LastActivityOn { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.IsAdmin)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.IsAdmin)]
#endif
	public bool IsAdmin { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.LastSignInAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.LastSignInAt)]
#endif
	public string LastSignInAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ProjectsLimit)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ProjectsLimit)]
#endif
	public int ProjectsLimit { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TwoFactorEnabled)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TwoFactorEnabled)]
#endif
	public bool TwoFactorEnabled { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Location)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Location)]
#endif
	public string Location { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Organization)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Organization)]
#endif
	public string Organization { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.State)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.State)]
#endif
	public string State { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Skype)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Skype)]
#endif
	public string Skype { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Twitter)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Twitter)]
#endif
	public string Twitter { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.WebUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.WebUrl)]
#endif
	public string WebUrl { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.WebsiteUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.WebsiteUrl)]
#endif
	public string WebsiteUrl { get; set; }
}
