using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace gitter.GitLab.Api
{
	[DataContract]
	class Project
	{
		[JsonProperty("id")]
		[DataMember]
		public long Id { get; set; }

		[JsonProperty("name")]
		[DataMember]
		public string Name { get; set; }

		[JsonProperty("name_with_namespace")]
		[DataMember]
		public string NameWithNamespace { get; set; }

		[JsonProperty("path")]
		[DataMember]
		public string Path { get; set; }

		[JsonProperty("path_with_namespace")]
		[DataMember]
		public string PathWithNamespace { get; set; }

		[JsonProperty("ssh_url_to_repo")]
		[DataMember]
		public string SshUrlToRepo { get; set; }

		[JsonProperty("http_url_to_repo")]
		[DataMember]
		public string HttpUrlToRepo { get; set; }

		public override string ToString() => NameWithNamespace;
	}
}
