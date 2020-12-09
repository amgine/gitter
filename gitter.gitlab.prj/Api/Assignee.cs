namespace gitter.GitLab.Api
{
	using System;
	using System.Runtime.Serialization;

	using Newtonsoft.Json;

	[DataContract]
	public sealed class Assignee
	{
		[JsonProperty("id")]
		[DataMember]
		public int Id { get; set; }

		[JsonProperty("state")]
		[DataMember]
		public string State { get; set; }

		[JsonProperty("avatar_url")]
		[DataMember]
		public string AvatarUrl { get; set; }

		[JsonProperty("name")]
		[DataMember]
		public string Name { get; set; }

		[JsonProperty("created_at")]
		[DataMember]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("username")]
		[DataMember]
		public string Username { get; set; }

		[JsonProperty("web_url")]
		[DataMember]
		public string WebUrl { get; set; }
	}
}
