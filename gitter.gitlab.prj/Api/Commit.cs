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
using System.Collections.Generic;
using System.Runtime.Serialization;
#if SYSTEM_TEXT_JSON
using System.Text.Json.Serialization;
#elif NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

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
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Id)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Id)]
#endif
	public string Id { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ShortId)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ShortId)]
#endif
	public string ShortId { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Title)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Title)]
#endif
	public string Title { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.AuthorName)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.AuthorName)]
#endif
	public string AuthorName { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.AuthorEmail)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.AuthorEmail)]
#endif
	public string AuthorEmail { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.AuthoredDate)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.AuthoredDate)]
#endif
	public DateTimeOffset AuthoredDate { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CommitterName)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CommitterName)]
#endif
	public string CommitterName { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CommitterEmail)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CommitterEmail)]
#endif
	public string CommitterEmail { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CommittedDate)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CommittedDate)]
#endif
	public DateTimeOffset CommittedDate { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.CreatedAt)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.CreatedAt)]
#endif
	public DateTimeOffset CreatedAt { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Message)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Message)]
#endif
	public string Message { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ParentIds)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ParentIds)]
#endif
	public string[] ParentIds { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.WebUrl)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.WebUrl)]
#endif
	public string WebUrl { get; set; }
}
