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
	[JsonPropertyName(Names.Status)]
	public TestCaseStatus Status { get; set; }

	[DataMember]
	[JsonPropertyName(Names.Name)]
	public string Name { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.ClassName)]
	public string? ClassName { get; set; }

	[DataMember]
	[JsonPropertyName(Names.File)]
	public string File { get; set; } = default!;

	[DataMember]
	[JsonPropertyName(Names.ExecutionTime)]
	public double ExecutionTime { get; set; }

	[DataMember]
	[JsonPropertyName(Names.SystemOutput)]
	public string? SystemOutput { get; set; }

	[DataMember]
	[JsonPropertyName(Names.StackTrace)]
	public string? StackTrace { get; set; }

	[DataMember]
	[JsonPropertyName(Names.RecentFailures)]
	public string? RecentFailures { get; set; }
}
