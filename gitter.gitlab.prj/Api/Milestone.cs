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
sealed class Milestone : ModifiableObject
{
	static class Names
	{
		public const string Description = @"description";
		public const string DueDate     = @"due_date";
		public const string StartDate   = @"start_date";
		public const string ProjectId   = @"project_id";
		public const string GroupId     = @"group_id";
		public const string Title       = @"title";
		public const string WebUrl      = @"web_url";
		public const string State       = @"state";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Description)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Description)]
#endif
	public string Description { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.DueDate)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.DueDate)]
#endif
	public string DueDate { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.StartDate)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.StartDate)]
#endif
	public string StartDate { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ProjectId)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ProjectId)]
#endif
	public int? ProjectId { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.GroupId)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.GroupId)]
#endif
	public int? GroupId { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Title)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Title)]
#endif
	public string Title { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.WebUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.WebUrl)]
#endif
	public string WebUrl { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.State)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.State)]
#endif
	public MilestoneState State { get; set; }
}
