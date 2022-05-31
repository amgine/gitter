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
class TestSuite
{
	static class Names
	{
		public const string Name         = @"name";
		public const string TotalTime    = @"total_time";
		public const string TotalCount   = @"total_count";
		public const string SuccessCount = @"success_count";
		public const string FailedCount  = @"failed_count";
		public const string SkippedCount = @"skipped_count";
		public const string ErrorCount   = @"error_count";
		public const string SuiteError   = @"suite_error";
		public const string TestCases    = @"test_cases";
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
	[JsonPropertyName(Names.TotalTime)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TotalTime)]
#endif
	public double TotalTime { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TotalCount)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TotalCount)]
#endif
	public int TotalCount { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.SuccessCount)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.Name)]
#endif
	public int SuccessCount { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.FailedCount)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.FailedCount)]
#endif
	public int FailedCount { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.SkippedCount)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.SkippedCount)]
#endif
	public int SkippedCount { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.ErrorCount)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.ErrorCount)]
#endif
	public int ErrorCount { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.SuiteError)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.SuiteError)]
#endif
	public string SuiteError { get; set; }

	[DataMember]
#if SYSTEM_TEXT_JSON
	[JsonPropertyName(Names.TestCases)]
#elif NEWTONSOFT_JSON
	[JsonProperty(Names.TestCases)]
#endif
	public TestCase[] TsstCases { get; set; }
}
