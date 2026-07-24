#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

#if SYSTEM_TEXT_JSON
[JsonConverter(typeof(WorkItemTypeConverter))]
#endif
enum WorkItemType
{
	Unknown = 0,

	[EnumMember(Value = @"issue")]
	Issue,

	[EnumMember(Value = @"incident")]
	Incident,

	[EnumMember(Value = @"test_case")]
	TestCase,

	[EnumMember(Value = @"task")]
	Task,

	[EnumMember(Value = @"requirement")]
	Requirement,

	[EnumMember(Value = @"objective")]
	Objective,

	[EnumMember(Value = @"key_result")]
	KeyResult,

	[EnumMember(Value = @"epic")]
	Epic,

	[EnumMember(Value = @"ticket")]
	Ticket,
}

static class WorkItemTypes
{
	public static readonly WorkItemType[] Filterable =
	[
		WorkItemType.Issue,
		WorkItemType.Incident,
		WorkItemType.TestCase,
		WorkItemType.Task,
	];

	public static string? ToApiString(WorkItemType value)
		=> value switch
		{
			WorkItemType.Issue       => @"issue",
			WorkItemType.Incident    => @"incident",
			WorkItemType.TestCase    => @"test_case",
			WorkItemType.Task        => @"task",
			WorkItemType.Requirement => @"requirement",
			WorkItemType.Objective   => @"objective",
			WorkItemType.KeyResult   => @"key_result",
			WorkItemType.Epic        => @"epic",
			WorkItemType.Ticket      => @"ticket",
			_ => default,
		};

	public static WorkItemType Parse(string? value)
	{
		if(string.IsNullOrWhiteSpace(value)) return WorkItemType.Unknown;

		return value!.Trim().Replace("_", "").Replace("-", "").ToLowerInvariant() switch
		{
			@"issue"       => WorkItemType.Issue,
			@"incident"    => WorkItemType.Incident,
			@"testcase"    => WorkItemType.TestCase,
			@"task"        => WorkItemType.Task,
			@"requirement" => WorkItemType.Requirement,
			@"objective"   => WorkItemType.Objective,
			@"keyresult"   => WorkItemType.KeyResult,
			@"epic"        => WorkItemType.Epic,
			@"ticket"      => WorkItemType.Ticket,
			_ => WorkItemType.Unknown,
		};
	}
}

#if SYSTEM_TEXT_JSON

sealed class WorkItemTypeConverter : JsonConverter<WorkItemType>
{
	public override WorkItemType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		=> reader.TokenType switch
		{
			JsonTokenType.String => WorkItemTypes.Parse(reader.GetString()),
			JsonTokenType.Null   => WorkItemType.Unknown,
			_ => WorkItemType.Unknown,
		};

	public override void Write(Utf8JsonWriter writer, WorkItemType value, JsonSerializerOptions options)
	{
		var str = WorkItemTypes.ToApiString(value);
		if(str is null) writer.WriteNullValue();
		else            writer.WriteStringValue(str);
	}
}

#endif
