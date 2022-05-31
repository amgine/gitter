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

using System.Runtime.Serialization;
#if SYSTEM_TEXT_JSON
using System.Text.Json.Serialization;
#endif

#if SYSTEM_TEXT_JSON
[JsonConverter(typeof(JsonStringEnumConverter))]
#endif
enum PipelineStatus
{
	[EnumMember(Value = @"running")]
	Running,
	[EnumMember(Value = @"pending")]
	Pending,
	[EnumMember(Value = @"success")]
	Success,
	[EnumMember(Value = @"failed")]
	Failed,
	[EnumMember(Value = @"canceled")]
	Canceled,
	[EnumMember(Value = @"skipped")]
	Skipped,
	[EnumMember(Value = @"preparing")]
	Preparing,
	[EnumMember(Value = @"scheduled")]
	Scheduled,
	[EnumMember(Value = @"created")]
	Created,
	[EnumMember(Value = @"manual")]
	Manual,
	[EnumMember(Value = @"waiting_for_resource")]
	WaitingForResource,
}
