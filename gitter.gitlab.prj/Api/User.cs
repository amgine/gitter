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

using Newtonsoft.Json;

namespace gitter.GitLab.Api
{
	[DataContract]
	sealed class User
	{
		[JsonProperty("current_sign_in_at")]
		[DataMember]
		public string CurrentSignInAt { get; set; }

		[JsonProperty("linkedin")]
		[DataMember]
		public string Linkedin { get; set; }

		[JsonProperty("can_create_project")]
		[DataMember]
		public bool CanCreateProject { get; set; }

		[JsonProperty("bio")]
		[DataMember]
		public string Bio { get; set; }

		[JsonProperty("avatar_url")]
		[DataMember]
		public string AvatarUrl { get; set; }

		[JsonProperty("can_create_group")]
		[DataMember]
		public bool CanCreateGroup { get; set; }

		[JsonProperty("confirmed_at")]
		[DataMember]
		public string ConfirmedAt { get; set; }

		[JsonProperty("color_scheme_id")]
		[DataMember]
		public int ColorSchemeId { get; set; }

		[JsonProperty("created_at")]
		[DataMember]
		public string CreatedAt { get; set; }

		[JsonProperty("identities")]
		[DataMember]
		public List<Identity> Identities { get; } = new List<Identity>();

		[JsonProperty("external")]
		[DataMember]
		public bool External { get; set; }

		[JsonProperty("email")]
		[DataMember]
		public string Email { get; set; }

		[JsonProperty("id")]
		[DataMember]
		public long Id { get; set; }

		[JsonProperty("last_activity_on")]
		[DataMember]
		public string LastActivityOn { get; set; }

		[JsonProperty("is_admin")]
		[DataMember]
		public bool IsAdmin { get; set; }

		[JsonProperty("last_sign_in_at")]
		[DataMember]
		public string LastSignInAt { get; set; }

		[JsonProperty("projects_limit")]
		[DataMember]
		public int ProjectsLimit { get; set; }

		[JsonProperty("two_factor_enabled")]
		[DataMember]
		public bool TwoFactorEnabled { get; set; }

		[JsonProperty("name")]
		[DataMember]
		public string Name { get; set; }

		[JsonProperty("location")]
		[DataMember]
		public string Location { get; set; }

		[JsonProperty("organization")]
		[DataMember]
		public string Organization { get; set; }

		[JsonProperty("state")]
		[DataMember]
		public string State { get; set; }

		[JsonProperty("skype")]
		[DataMember]
		public string Skype { get; set; }

		[JsonProperty("twitter")]
		[DataMember]
		public string Twitter { get; set; }

		[JsonProperty("web_url")]
		[DataMember]
		public string WebUrl { get; set; }

		[JsonProperty("username")]
		[DataMember]
		public string Username { get; set; }

		[JsonProperty("website_url")]
		[DataMember]
		public string WebsiteUrl { get; set; }
	}
}
