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
	[JsonPropertyName(Names.Id)]
	public long Id { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Status)]
	public string Status { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Stage)]
	public string Stage { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Ref)]
	public string Ref { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Tag)]
	public bool Tag { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Coverage)]
	public string? Coverage { get; set; }

	[DataMember]
	[JsonPropertyName(Names.AllowFailure)]
	public bool AllowFailure { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CreatedAt)]
	public DateTimeOffset CreatedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.StartedAt)]
	public DateTimeOffset? StartedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.FinishedAt)]
	public DateTimeOffset? FinishedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Duration)]
	public double Duration { get; set; }

	[DataMember]
	[JsonPropertyName(Names.QueuedDuration)]
	public double QueuedDuration { get; set; }

	[DataMember]
	[JsonPropertyName(Names.User)]
	public User User { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Commit)]
	public Commit Commit { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Pipeline)]
	public Pipeline Pipeline { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public string WebUrl { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Runner)]
	public Runner Runner { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.ArtifactsExpireAt)]
	public DateTimeOffset? ArtifactsExpireAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.TagList)]
	public string[]? TagList { get; set; }
}
