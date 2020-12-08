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
	using System.Runtime.Serialization;

	using Newtonsoft.Json;

	[DataContract]
	sealed class Job
	{
		[JsonProperty("id")]
		[DataMember]
		public long Id { get; set; }

		[JsonProperty("status")]
		[DataMember]
		public string Status { get; set; }

		[JsonProperty("stage")]
		[DataMember]
		public string Stage { get; set; }

		[JsonProperty("name")]
		[DataMember]
		public string Name { get; set; }

		[JsonProperty("ref")]
		[DataMember]
		public string Ref { get; set; }

		[JsonProperty("tag")]
		[DataMember]
		public bool Tag { get; set; }

		/*
		coverage
		 */

		[JsonProperty("allow_failure")]
		[DataMember]
		public bool AllowFailure { get; set; }

		[JsonProperty("created_at")]
		[DataMember]
		public DateTimeOffset CreatedAt { get; set; }

		[JsonProperty("started_at")]
		[DataMember]
		public DateTimeOffset? StartedAt { get; set; }

		[JsonProperty("finished_at")]
		[DataMember]
		public DateTimeOffset? FinishedAt { get; set; }

		[JsonProperty("duration")]
		[DataMember]
		public double Duration { get; set; }

		[JsonProperty("user")]
		[DataMember]
		public User User { get; set; }

		[JsonProperty("commit")]
		[DataMember]
		public Commit Commit { get; set; }

		[JsonProperty("pipeline")]
		[DataMember]
		public Pipeline Pipeline { get; set; }

		[JsonProperty("web_url")]
		[DataMember]
		public string WebUrl { get; set; }

		/*artifacts*/

		[JsonProperty("runner")]
		[DataMember]
		public Runner Runner { get; set; }

		[JsonProperty("artifacts_expire_at")]
		[DataMember]
		public DateTimeOffset? ArtifactsExpireAt { get; set; }
	}
}
