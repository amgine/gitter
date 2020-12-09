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
    sealed class Commit
    {
        [JsonProperty("id")]
        [DataMember]
        public string Id { get; set; }

        [JsonProperty("short_id")]
        [DataMember]
        public string ShortId { get; set; }

        [JsonProperty("title")]
        [DataMember]
        public string Title { get; set; }

        [JsonProperty("author_name")]
        [DataMember]
        public string AuthorName { get; set; }

        [JsonProperty("author_email")]
        [DataMember]
        public string AuthorEmail { get; set; }

        [JsonProperty("authored_date")]
        [DataMember]
        public DateTimeOffset AuthoredDate { get; set; }

        [JsonProperty("committer_name")]
        [DataMember]
        public string CommitterName { get; set; }

        [JsonProperty("committer_email")]
        [DataMember]
        public string CommitterEmail { get; set; }

        [JsonProperty("committed_date")]
        [DataMember]
        public DateTimeOffset CommittedDate { get; set; }

        [JsonProperty("created_at")]
        [DataMember]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("message")]
        [DataMember]
        public string Message { get; set; }

        [JsonProperty("parent_ids")]
        [DataMember]
        public string[] ParentIds { get; set; }

        [JsonProperty("web_url")]
        [DataMember]
        public string WebUrl { get; set; }
    }
}
