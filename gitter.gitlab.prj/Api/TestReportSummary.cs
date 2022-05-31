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
internal class TestReportSummary
{
	static class Names
	{
		public const string Total  = @"total";
		public const string Suites = @"test_suites";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Total)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Total)]
#endif
	public TestReportSummaryTotal Total { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Suites)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Suites)]
#endif
	public TestReportSummarySuite[] Suites { get; set; }
}

[DataContract]
class TestReportSummaryTotal
{
	static class Names
	{
		public const string Time       = @"time";
		public const string Count      = @"count";
		public const string Success    = @"success";
		public const string Failed     = @"failed";
		public const string Skipped    = @"skipped";
		public const string Error      = @"error";
		public const string SuiteError = @"suite_error";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Time)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Time)]
#endif
	public double Time { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Count)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Count)]
#endif
	public int Count { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Success)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Success)]
#endif
	public int Success { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Failed)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Failed)]
#endif
	public int Failed { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Skipped)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Skipped)]
#endif
	public int Skipped { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Error)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Error)]
#endif
	public int Error { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.SuiteError)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.SuiteError)]
#endif
	public string SuiteError { get; set; }
}

[DataContract]
class TestReportSummarySuite
{
	static class Names
	{
		public const string Name       = @"name";
		public const string Time       = @"total_time";
		public const string Count      = @"total_count";
		public const string Success    = @"success_count";
		public const string Failed     = @"failed_count";
		public const string Skipped    = @"skipped_count";
		public const string Error      = @"error_count";
		public const string BuildIds   = @"build_ids";
		public const string SuiteError = @"suite_error";
	}

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Name)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Name)]
#endif
	public string Name { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Time)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Time)]
#endif
	public double Time { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Count)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Count)]
#endif
	public int Count { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Success)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Success)]
#endif
	public int Success { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Failed)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Failed)]
#endif
	public int Failed { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Skipped)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Skipped)]
#endif
	public int Skipped { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.Error)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Error)]
#endif
	public int Error { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.BuildIds)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.BuildIds)]
#endif
	public long[] BuildIds { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.SuiteError)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.SuiteError)]
#endif
	public string SuiteError { get; set; }
}
