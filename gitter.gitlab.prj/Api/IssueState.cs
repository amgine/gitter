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
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

#if SYSTEM_TEXT_JSON
[JsonConverter(typeof(IssueStateConverter))]
#endif
enum IssueState
{
	Unknown = 0,

	[EnumMember(Value = @"opened")]
	Opened,

	[EnumMember(Value = @"closed")]
	Closed,
}

#if SYSTEM_TEXT_JSON

sealed class IssueStateConverter : JsonConverter<IssueState>
{
	public override IssueState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if(reader.TokenType != JsonTokenType.String) return IssueState.Unknown;

		return reader.GetString()?.Trim().ToLowerInvariant() switch
		{
			@"opened"   => IssueState.Opened,
			@"reopened" => IssueState.Opened,
			@"closed"   => IssueState.Closed,
			_ => IssueState.Unknown,
		};
	}

	public override void Write(Utf8JsonWriter writer, IssueState value, JsonSerializerOptions options)
	{
		switch(value)
		{
			case IssueState.Opened: writer.WriteStringValue(@"opened"); break;
			case IssueState.Closed: writer.WriteStringValue(@"closed"); break;
			default: writer.WriteNullValue(); break;
		}
	}
}

#endif
