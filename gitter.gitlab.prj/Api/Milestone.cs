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
	[JsonPropertyName(Names.Description)]
	public string? Description { get; set; }

	[DataMember]
	[JsonPropertyName(Names.DueDate)]
	public string? DueDate { get; set; }

	[DataMember]
	[JsonPropertyName(Names.StartDate)]
	public string StartDate { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.ProjectId)]
	public int? ProjectId { get; set; }

	[DataMember]
	[JsonPropertyName(Names.GroupId)]
	public int? GroupId { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Title)]
	public string Title { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public string WebUrl { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.State)]
	public MilestoneState State { get; set; }
}
