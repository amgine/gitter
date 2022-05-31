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
sealed class Job
{
	static class Names
	{
		public const string Id                = @"id";
		public const string Status            = @"status";
		public const string Stage             = @"stage";
		public const string Name              = @"name";
		public const string Ref               = @"ref";
		public const string Tag               = @"tag";
		public const string Coverage          = @"coverage";
		public const string AllowFailure      = @"allow_failure";
		public const string CreatedAt         = @"created_at";
		public const string StartedAt         = @"started_at";
		public const string FinishedAt        = @"finished_at";
		public const string Duration          = @"duration";
		public const string QueuedDuration    = @"queued_duration";
		public const string User              = @"user";
		public const string Commit            = @"commit";
		public const string Pipeline          = @"pipeline";
		public const string WebUrl            = @"web_url";
		public const string Runner            = @"runner";
		public const string ArtifactsExpireAt = @"artifacts_expire_at";
		public const string TagList           = @"tag_list";
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
	[JsonPropertyName(Names.Status)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Status)]
#endif
	public string Status { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Stage)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Stage)]
#endif
	public string Stage { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Name)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Name)]
#endif
	public string Name { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Ref)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Ref)]
#endif
	public string Ref { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Tag)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Tag)]
#endif
	public bool Tag { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Coverage)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Coverage)]
#endif
	public string Coverage { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.AllowFailure)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.AllowFailure)]
#endif
	public bool AllowFailure { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CreatedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CreatedAt)]
#endif
	public DateTimeOffset CreatedAt { get; set; }

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
	public double QueuedDuration { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.User)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.User)]
#endif
	public User User { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Commit)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Commit)]
#endif
	public Commit Commit { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Pipeline)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Pipeline)]
#endif
	public Pipeline Pipeline { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.WebUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.WebUrl)]
#endif
	public string WebUrl { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Runner)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Runner)]
#endif
	public Runner Runner { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ArtifactsExpireAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ArtifactsExpireAt)]
#endif
	public DateTimeOffset? ArtifactsExpireAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TagList)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TagList)]
#endif
	public string[] TagList { get; set; }
}
