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
	private static string ScopeToString(JobScope scope)
		=> scope switch
		{
			JobScope.Created  => @"created",
			JobScope.Pending  => @"pending",
			JobScope.Running  => @"running",
			JobScope.Failed   => @"failed",
			JobScope.Success  => @"success",
			JobScope.Canceled => @"canceled",
			JobScope.Skipped  => @"skipped",
			JobScope.Manual   => @"manual",
			_ => throw new ArgumentException($"Unknown scope: {scope}", nameof(scope)),
		};

	public Task<IReadOnlyList<Job>> GetJobsAsync(NameOrNumericId projectId, JobScope? scope = default,
		CancellationToken cancellationToken = default)
	{
		var urlBuilder = new StringBuilder();
		AppendProjectUrl(urlBuilder, projectId);
		urlBuilder.Append("/jobs");

		var sep = '?';

		if(scope.HasValue) AppendParameter(urlBuilder, ref sep, @"scope", ScopeToString(scope.Value));

		return ReadPagedResultAsync<Job>(urlBuilder.ToString(), cancellationToken);
	}

	public Task<IReadOnlyList<Job>> GetJobsAsync(NameOrNumericId projectId, long pipelineId, JobScope? scope = default,
		CancellationToken cancellationToken = default)
	{
		var urlBuilder = new StringBuilder();
		AppendPipelineUrl(urlBuilder, projectId, pipelineId);
		urlBuilder.Append("/jobs");

		var sep = '?';

		if(scope.HasValue) AppendParameter(urlBuilder, ref sep, @"scope", ScopeToString(scope.Value));

		return ReadPagedResultAsync<Job>(urlBuilder.ToString(), cancellationToken);
	}
}
