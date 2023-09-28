﻿#region Copyright Notice
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

using System.Runtime.Serialization;
#if SYSTEM_TEXT_JSON
using System.Text.Json.Serialization;
#endif

#if SYSTEM_TEXT_JSON
[JsonConverter(typeof(JsonStringEnumConverter))]
#endif
enum PipelineSource
{
	[EnumMember(Value = @"push")]
	Push,
	[EnumMember(Value = @"web")]
	Web,
	[EnumMember(Value = @"trigger")]
	Trigger,
	[EnumMember(Value = @"schedule")]
	Schedule,
	[EnumMember(Value = @"api")]
	Api,
	[EnumMember(Value = @"external")]
	External,
	[EnumMember(Value = @"pipeline")]
	Pipeline,
	[EnumMember(Value = @"chat")]
	Chat,
	[EnumMember(Value = @"webide")]
	WebIDE,
	[EnumMember(Value = @"merge_request_event")]
	MergeRequestEvent,
	[EnumMember(Value = @"external_pull_request_event")]
	ExternalPullRequestEvent,
	[EnumMember(Value = @"parent_pipeline")]
	ParentPipeline,
	[EnumMember(Value = @"ondemand_dast_scan")]
	OnDemandDastScan,
	[EnumMember(Value = @"ondemand_dast_validation")]
	OnDemandDastValidation,
}