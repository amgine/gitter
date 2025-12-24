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

[DataContract]
internal class TestReportSummary
{
	static class Names
	{
		public const string Total  = @"total";
		public const string Suites = @"test_suites";
	}

	[DataMember]
	[JsonPropertyName(Names.Total)]
	public TestReportSummaryTotal? Total { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Suites)]
	public TestReportSummarySuite[]? Suites { get; set; }
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
	[JsonPropertyName(Names.Time)]
	public double Time { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Count)]
	public int Count { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Success)]
	public int Success { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Failed)]
	public int Failed { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Skipped)]
	public int Skipped { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Error)]
	public int Error { get; set; }

	[DataMember]
	[JsonPropertyName(Names.SuiteError)]
	public string? SuiteError { get; set; }
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
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.Time)]
	public double Time { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Count)]
	public int Count { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Success)]
	public int Success { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Failed)]
	public int Failed { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Skipped)]
	public int Skipped { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Error)]
	public int Error { get; set; }

	[DataMember]
	[JsonPropertyName(Names.BuildIds)]
	public long[]? BuildIds { get; set; }

	[DataMember]
	[JsonPropertyName(Names.SuiteError)]
	public string? SuiteError { get; set; }
}
