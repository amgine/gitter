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
class IssueTimeStatistic
{
	static class Names
	{
		public const string TimeEstimate        = @"time_estimate";
		public const string TotalTimeSpent      = @"total_time_spent";
		public const string HumanTimeEstimate   = @"human_time_estimate";
		public const string HumanTotalTimeSpent = @"human_total_time_spent";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TimeEstimate)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TimeEstimate)]
#endif
	public int TimeEstimate { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TotalTimeSpent)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TotalTimeSpent)]
#endif
	public int TotalTimeSpent { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.HumanTimeEstimate)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.HumanTimeEstimate)]
#endif
	public string HumanTimeEstimate { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.HumanTotalTimeSpent)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.HumanTotalTimeSpent)]
#endif
	public string HumanTotalTimeSpent { get; set; }
}
