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
	[JsonPropertyName(Names.Confidential)]
	public bool Confidential { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Assignees)]
	public Assignee[]? Assignees { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Assignee)]
	public Assignee? Assignee { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Author)]
	public Assignee Author { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Description)]
	public string Description { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.DueDate)]
	public string? DueDate { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.ProjectId)]
	public long ProjectId { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Labels)]
	public string[]? Labels { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Milestone)]
	public Milestone? Milestone { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ClosedAt)]
	public DateTime? ClosedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ClosedBy)]
	public ClosedBy? ClosedBy { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Title)]
	public string Title { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.UserNotesCount)]
	public int UserNotesCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.State)]
	public IssueState State { get; set; }

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public string WebUrl { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Weight)]
	public int? Weight { get; set; }

	[DataMember]
	[JsonPropertyName(Names.TimeStats)]
	public IssueTimeStatistic? TimeStats { get; set; }

	[DataMember]
	[JsonPropertyName(Names.TaskCompletionStatus)]
	public IssueTaskCompletionStatus? TaskCompletionStatus { get; set; }
}
