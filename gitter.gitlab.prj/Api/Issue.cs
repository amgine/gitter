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
sealed class Issue : ModifiableObject
{
	static class Names
	{
		public const string Confidential   = @"confidential";
		public const string Assignees      = @"assignees";
		public const string Assignee       = @"assignee";
		public const string Author         = @"author";
		public const string Description    = @"description";
		public const string DueDate        = @"due_date";
		public const string ProjectId      = @"project_id";
		public const string Labels         = @"labels";
		public const string Milestone      = @"milestone";
		public const string ClosedAt       = @"closed_at";
		public const string ClosedBy       = @"closed_by";
		public const string Title          = @"title";
		public const string UserNotesCount = @"user_notes_count";
		public const string State          = @"state";
		public const string WebUrl         = @"web_url";
		public const string Weight         = @"weight";
		public const string TimeStats      = @"time_stats";
		public const string TaskCompletionStatus = @"task_completion_status";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Confidential)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Confidential)]
#endif
	public bool Confidential { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Assignees)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Assignees)]
#endif
	public Assignee[] Assignees { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Assignee)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Assignee)]
#endif
	public Assignee Assignee { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Author)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Author)]
#endif
	public Assignee Author { get; set; }

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
	[JsonPropertyName(Names.ProjectId)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ProjectId)]
#endif
	public long ProjectId { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Labels)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Labels)]
#endif
	public string[] Labels { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Milestone)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Milestone)]
#endif
	public Milestone Milestone { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ClosedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ClosedAt)]
#endif
	public DateTime? ClosedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ClosedBy)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ClosedBy)]
#endif
	public ClosedBy ClosedBy { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Title)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Title)]
#endif
	public string Title { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.UserNotesCount)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.UserNotesCount)]
#endif
	public int UserNotesCount { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.State)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.State)]
#endif
	public IssueState State { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.WebUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.WebUrl)]
#endif
	public string WebUrl { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Weight)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Weight)]
#endif
	public int? Weight { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TimeStats)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TimeStats)]
#endif
	public IssueTimeStatistic TimeStats { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TaskCompletionStatus)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TaskCompletionStatus)]
#endif
	public IssueTaskCompletionStatus TaskCompletionStatus { get; set; }
}
