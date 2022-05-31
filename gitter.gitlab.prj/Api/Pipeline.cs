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
#if SYSTEM_TEXT_JSON
using System.Text.Json.Serialization;
#elif NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

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
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Id)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Id)]
#endif
	public long Id { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Iid)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Iid)]
#endif
	public long Iid { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ProjectId)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ProjectId)]
#endif
	public long ProjectId { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Sha)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Sha)]
#endif
	public string Sha { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Ref)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Ref)]
#endif
	public string Ref { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Status)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Status)]
#endif
	public PipelineStatus Status { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Source)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Source)]
#endif
	public PipelineSource Source { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CreatedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CreatedAt)]
#endif
	public DateTimeOffset? CreatedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.UpdatedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.UpdatedAt)]
#endif
	public DateTimeOffset? UpdatedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.WebUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.WebUrl)]
#endif
	public Uri WebUrl { get; set; }
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
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.BeforeSha)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.BeforeSha)]
#endif
	public string BeforeSha { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Tag)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Tag)]
#endif
	public bool Tag { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.YamlErrors)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.YamlErrors)]
#endif
	public string YamlErrors { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.User)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.User)]
#endif
	public User User { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.StartedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.StartedAt)]
#endif
	public DateTimeOffset? StartedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.FinishedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.FinishedAt)]
#endif
	public DateTimeOffset? FinishedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CommittedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CommittedAt)]
#endif
	public DateTimeOffset? CommittedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Duration)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Duration)]
#endif
	public double Duration { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.QueuedDuration)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.QueuedDuration)]
#endif
	public string QueuedDuration { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Coverage)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Coverage)]
#endif
	public string Coverage { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.DetailedStatus)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.DetailedStatus)]
#endif
	public PipelineDetailedStatus DetailedStatus { get; set; }
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
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Icon)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Icon)]
#endif
	public string Icon { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Text)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Text)]
#endif
	public string Text { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Label)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Label)]
#endif
	public string Label { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Group)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Group)]
#endif
	public string Group { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ToolTip)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ToolTip)]
#endif
	public string ToolTip { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.HasDetails)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.HasDetails)]
#endif
	public bool HasDetails { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.DetailsPath)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.DetailsPath)]
#endif
	public string DetailsPath { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Illustration)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Illustration)]
#endif
	public string Illustration { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.FavIcon)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.FavIcon)]
#endif
	public string FavIcon { get; set; }
}
