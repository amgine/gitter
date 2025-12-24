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
using System.Text.Json.Serialization;

[DataContract]
class TestReport
{
	static class Names
	{
		public const string TotalTime    = @"total_time";
		public const string TotalCount   = @"total_count";
		public const string SuccessCount = @"success_count";
		public const string FailedCount  = @"failed_count";
		public const string SkippedCount = @"skipped_count";
		public const string ErrorCount   = @"error_count";
		public const string TestSuites   = @"test_suites";
	}

	[DataMember]
	[JsonPropertyName(Names.TotalTime)]
	public double TotalTime { get; set; }

	[DataMember]
	[JsonPropertyName(Names.TotalCount)]
	public int TotalCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.SuccessCount)]
	public int SuccessCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.FailedCount)]
	public int FailedCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.SkippedCount)]
	public int SkippedCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.ErrorCount)]
	public int ErrorCount { get; set; }

	[DataMember]
	[JsonPropertyName(Names.TestSuites)]
	public TestSuite[]? TestSuites { get; set; }
}
