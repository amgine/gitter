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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

partial class ApiEndpoint
{
	private static string ScopeToString(PipelineScope scope)
		=> scope switch
		{
			PipelineScope.Tags     => @"tags",
			PipelineScope.Branches => @"branches",
			PipelineScope.Pending  => @"pending",
			PipelineScope.Running  => @"running",
			PipelineScope.Finished => @"finished",
			_ => throw new ArgumentException("Unknown scope.", nameof(scope)),
		};

	private static string StatusToString(PipelineStatus status)
		=> status switch
		{
			PipelineStatus.Created            => @"created",
			PipelineStatus.WaitingForResource => @"waiting_for_resource",
			PipelineStatus.Preparing          => @"preparing",
			PipelineStatus.Pending            => @"pending",
			PipelineStatus.Running            => @"running",
			PipelineStatus.Success            => @"success",
			PipelineStatus.Failed             => @"failed",
			PipelineStatus.Canceled           => @"canceled",
			PipelineStatus.Skipped            => @"skipped",
			PipelineStatus.Manual             => @"manual",
			PipelineStatus.Scheduled          => @"scheduled",
			_ => throw new ArgumentException("Unknown status.", nameof(status)),
		};

	private static string SourceToString(PipelineSource source)
		=> source switch
		{
			PipelineSource.Push                     => @"push",
			PipelineSource.Web                      => @"web",
			PipelineSource.Trigger                  => @"trigger",
			PipelineSource.Schedule                 => @"schedule",
			PipelineSource.Api                      => @"api",
			PipelineSource.External                 => @"external",
			PipelineSource.Pipeline                 => @"pipeline",
			PipelineSource.Chat                     => @"chat",
			PipelineSource.WebIDE                   => @"webide",
			PipelineSource.MergeRequestEvent        => @"merge_request_event",
			PipelineSource.ExternalPullRequestEvent => @"external_pull_request_event",
			PipelineSource.ParentPipeline           => @"parent_pipeline",
			PipelineSource.OnDemandDastScan         => @"ondemand_dast_scan",
			PipelineSource.OnDemandDastValidation   => @"ondemand_dast_validation",
			_ => throw new ArgumentException("Unknown source.", nameof(source)),
		};

	private static string OrderToString(PipelineOrder order)
		=> order switch
		{
			PipelineOrder.Id        => @"id",
			PipelineOrder.Status    => @"status",
			PipelineOrder.Ref       => @"ref",
			PipelineOrder.UpdatedAt => @"updated_at",
			PipelineOrder.UserId    => @"user_id",
			_ => throw new ArgumentException("Unknown order.", nameof(order)),
		};

	private static void AppendPipelineUrl(StringBuilder urlBuilder, NameOrNumericId projectId, long pipelineId)
	{
		AppendProjectUrl(urlBuilder, projectId, @"pipelines");
		urlBuilder.Append('/');
		urlBuilder.Append(pipelineId.ToString(CultureInfo.InvariantCulture));
	}

	public Task<IReadOnlyList<Pipeline>> GetPipelinesAsync(
		NameOrNumericId projectId,
		string?         sha           = default,
		string?         reference     = default,
		PipelineScope?  scope         = default,
		PipelineStatus? status        = default,
		PipelineSource? source        = default,
		string?         username      = default,
		DateTimeOffset? updatedBefore = default,
		DateTimeOffset? updatedAfter  = default,
		PipelineOrder?  orderBy       = default,
		SortOrder?      sort          = default,
		CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendProjectUrl(query, projectId, @"pipelines");

		var sep = '?';

		if(scope.HasValue)         AppendParameter(query, ref sep, @"scope",          ScopeToString(scope.Value));
		if(status.HasValue)        AppendParameter(query, ref sep, @"status",         StatusToString(status.Value));
		if(source.HasValue)        AppendParameter(query, ref sep, @"source",         SourceToString(source.Value));
		if(reference is not null)  AppendParameter(query, ref sep, @"ref",            Uri.EscapeDataString(reference));
		if(sha is not null)        AppendParameter(query, ref sep, @"sha",            sha);
		if(username is not null)   AppendParameter(query, ref sep, @"username",       Uri.EscapeDataString(username));
		if(updatedAfter.HasValue)  AppendParameter(query, ref sep, @"updated_after",  DateTimeHelper.FormatISO8601(updatedAfter.Value));
		if(updatedBefore.HasValue) AppendParameter(query, ref sep, @"updated_before", DateTimeHelper.FormatISO8601(updatedBefore.Value));
		if(orderBy.HasValue)       AppendParameter(query, ref sep, @"order_by",       OrderToString(orderBy.Value));
		if(sort.HasValue)          AppendParameter(query, ref sep, @"sort",           SortToString(sort.Value));

		return ReadPagedResultAsync<Pipeline>(query.ToString(), cancellationToken);
	}

	public Task<PipelineEx?> GetPipelineAsync(NameOrNumericId projectId, long pipelineId,
		CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendPipelineUrl(query, projectId, pipelineId);

		return GetAsync<PipelineEx>(query.ToString(), cancellationToken);
	}

	private Task<T?> GetPipelineInfoAsync<T>(NameOrNumericId projectId, long pipelineId, string suffix,
		CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendPipelineUrl(query, projectId, pipelineId);
		query.Append('/');
		query.Append(suffix);

		return GetAsync<T>(query.ToString(), cancellationToken);
	}

	public Task<TestReport?> GetTestReportAsync(NameOrNumericId projectId, long pipelineId,
		CancellationToken cancellationToken = default)
	{
		return GetPipelineInfoAsync<TestReport>(
			projectId, pipelineId, @"test_report", cancellationToken);
	}

	public Task<TestReportSummary?> GetTestReportSummaryAsync(NameOrNumericId projectId, long pipelineId,
		CancellationToken cancellationToken = default)
	{
		return GetPipelineInfoAsync<TestReportSummary>(
			projectId, pipelineId, @"test_report_summary", cancellationToken);
	}

	public Task DeletePipelineAsync(NameOrNumericId projectId, long pipelineId,
		CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendPipelineUrl(query, projectId, pipelineId);

		return DeleteAsync(query.ToString(), cancellationToken);
	}
}
