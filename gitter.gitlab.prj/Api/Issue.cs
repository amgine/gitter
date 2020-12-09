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

namespace gitter.GitLab.Api
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using Newtonsoft.Json;

	[DataContract]
	sealed class Issue : ModifiableObject
	{
		[JsonProperty("confidential")]
		[DataMember]
		public bool Confidential { get; set; }

		[JsonProperty("assignees")]
		[DataMember]
		public Assignee[] Assignees { get; set; }

		[JsonProperty("assignee")]
		[DataMember]
		public Assignee Assignee { get; set; }

		[JsonProperty("author")]
		[DataMember]
		public Assignee Author { get; set; }

		[JsonProperty("description")]
		[DataMember]
		public string Description { get; set; }

		[JsonProperty("due_date")]
		[DataMember]
		public string DueDate { get; set; }

		[JsonProperty("project_id")]
		[DataMember]
		public string ProjectId { get; set; }

		[JsonProperty("labels")]
		[DataMember]
		public string[] Labels { get; set; }

		[JsonProperty("milestone")]
		[DataMember]
		public Milestone Milestone { get; set; }

		[JsonProperty("closed_at")]
		[DataMember]
		public DateTime? ClosedAt { get; set; }

		[JsonProperty("closed_by")]
		[DataMember]
		public ClosedBy ClosedBy { get; set; }

		[JsonProperty("title")]
		[DataMember]
		public string Title { get; set; }

		[JsonProperty("user_notes_count")]
		[DataMember]
		public int UserNotesCount { get; set; }

		[JsonProperty("state")]
		[DataMember]
		public IssueState State { get; set; }

		[JsonProperty("web_url")]
		[DataMember]
		public string WebUrl { get; set; }

		[JsonProperty("weight")]
		[DataMember]
		public int? Weight { get; set; }

		[JsonProperty("time_stats")]
		[DataMember]
		public IssueTimeStatistic TimeStats { get; set; }

		[JsonProperty("task_completion_status")]
		[DataMember]
		public IssueTaskCompletionStatus TaskCompletionStatus { get; set; }
	}
}
