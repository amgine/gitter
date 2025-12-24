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
sealed class Commit
{
	static class Names
	{
		public const string Id             = @"id";
		public const string ShortId        = @"short_id";
		public const string Title          = @"title";
		public const string AuthorName     = @"author_name";
		public const string AuthorEmail    = @"author_email";
		public const string AuthoredDate   = @"authored_date";
		public const string CommitterName  = @"committer_name";
		public const string CommitterEmail = @"committer_email";
		public const string CommittedDate  = @"committed_date";
		public const string CreatedAt      = @"created_at";
		public const string Message        = @"message";
		public const string ParentIds      = @"parent_ids";
		public const string WebUrl         = @"web_url";
	}

	[DataMember]
	[JsonPropertyName(Names.Id)]
	public string Id { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.ShortId)]
	public string ShortId { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Title)]
	public string Title { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.AuthorName)]
	public string AuthorName { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.AuthorEmail)]
	public string AuthorEmail { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.AuthoredDate)]
	public DateTimeOffset AuthoredDate { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CommitterName)]
	public string CommitterName { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.CommitterEmail)]
	public string CommitterEmail { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.CommittedDate)]
	public DateTimeOffset CommittedDate { get; set; }

	[DataMember]
	[JsonPropertyName(Names.CreatedAt)]
	public DateTimeOffset CreatedAt { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Message)]
	public string Message { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.ParentIds)]
	public string[] ParentIds { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.WebUrl)]
	public string WebUrl { get; set; } = default!;
}
