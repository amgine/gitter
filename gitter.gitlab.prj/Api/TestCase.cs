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
class TestCase
{
	static class Names
	{
		public const string Status         = @"status";
		public const string Name           = @"name";
		public const string ClassName      = @"classname";
		public const string File           = @"file";
		public const string ExecutionTime  = @"execution_time";
		public const string SystemOutput   = @"system_output";
		public const string StackTrace     = @"stack_trace";
		public const string RecentFailures = @"recent_failures";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Status)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Status)]
#endif
	public TestCaseStatus Status { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Name)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Name)]
#endif
	public string Name { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ClassName)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ClassName)]
#endif
	public string ClassName { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.File)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.File)]
#endif
	public string File { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ExecutionTime)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ExecutionTime)]
#endif
	public double ExecutionTime { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.SystemOutput)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.SystemOutput)]
#endif
	public string SystemOutput { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.StackTrace)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.StackTrace)]
#endif
	public string StackTrace { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.RecentFailures)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.RecentFailures)]
#endif
	public string RecentFailures { get; set; }
}
