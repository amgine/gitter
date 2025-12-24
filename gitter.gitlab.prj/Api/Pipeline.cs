#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
class Pipeline
{
	static class Names
	{
		public const string Id        = @"id";
		public const string Iid       = @"iid";
		public const string ProjectId = @"project_id";
		public const string Sha       = @"sha";
		public const string Ref       = @"ref";
		public const string Status    = @"status";
		public const string Source    = @"source";
		public const string CreatedAt = @"created_at";
		public const string UpdatedAt = @"updated_at";
		public const string WebUrl    = @"web_url";
	}

	[DataMember(IsRequired = true)]
	[JsonPropertyName(Names.Id)]
	public long Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Iid)]
	public long Iid { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ProjectId)]
	public long ProjectId { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Sha)]
	public string Sha { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Ref)]
	public string Ref { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Status)]
	public PipelineStatus Status { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Source)]
	public PipelineSource Source { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CreatedAt)]
	public DateTimeOffset? CreatedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.UpdatedAt)]
	public DateTimeOffset? UpdatedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public Uri WebUrl { get; set; } = default!;
}

[DataContract]
class PipelineEx : Pipeline
{
	static class Names
	{
		public const string BeforeSha      = @"before_sha";
		public const string Tag            = @"tag";
		public const string YamlErrors     = @"yaml_errors";
		public const string User           = @"user";
		public const string StartedAt      = @"started_at";
		public const string FinishedAt     = @"finished_at";
		public const string CommittedAt    = @"committed_at";
		public const string Duration       = @"duration";
		public const string QueuedDuration = @"queued_duration";
		public const string Coverage       = @"coverage";
		public const string DetailedStatus = @"detailed_status";
	}

	[DataMember]
	[JsonPropertyName(Names.BeforeSha)]
	public string BeforeSha { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Tag)]
	public bool Tag { get; set; }

	[DataMember]
	[JsonPropertyName(Names.YamlErrors)]
	public string? YamlErrors { get; set; }

	[DataMember]
	[JsonPropertyName(Names.User)]
	public User User { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.StartedAt)]
	public DateTimeOffset? StartedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.FinishedAt)]
	public DateTimeOffset? FinishedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CommittedAt)]
	public DateTimeOffset? CommittedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Duration)]
	public double Duration { get; set; }

	[DataMember]
	[JsonPropertyName(Names.QueuedDuration)]
	public string? QueuedDuration { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Coverage)]
	public string? Coverage { get; set; }

	[DataMember]
	[JsonPropertyName(Names.DetailedStatus)]
	public PipelineDetailedStatus? DetailedStatus { get; set; }
}

[DataContract]
sealed class PipelineDetailedStatus
{
	static class Names
	{
		public const string Icon         = @"icon";
		public const string Text         = @"text";
		public const string Label        = @"label";
		public const string Group        = @"group";
		public const string ToolTip      = @"tooltip";
		public const string HasDetails   = @"has_details";
		public const string DetailsPath  = @"details_path";
		public const string Illustration = @"illustration";
		public const string FavIcon      = @"favicon";
	}

	[DataMember]
	[JsonPropertyName(Names.Icon)]
	public string? Icon { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Text)]
	public string? Text { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Label)]
	public string? Label { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Group)]
	public string? Group { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ToolTip)]
	public string? ToolTip { get; set; }

	[DataMember]
	[JsonPropertyName(Names.HasDetails)]
	public bool HasDetails { get; set; }

	[DataMember]
	[JsonPropertyName(Names.DetailsPath)]
	public string? DetailsPath { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Illustration)]
	public string? Illustration { get; set; }

	[DataMember]
	[JsonPropertyName(Names.FavIcon)]
	public string? FavIcon { get; set; }
}
