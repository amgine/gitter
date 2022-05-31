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
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

partial class ApiEndpoint
{
	private static string StateToString(IssueState state)
		=> state switch
		{
			IssueState.Opened => @"opened",
			IssueState.Closed => @"closed",
			_ => throw new ArgumentException($"Unknown state: {state}.", nameof(state)),
		};

	private static string ScopeToString(IssueScope scope)
		=> scope switch
		{
			IssueScope.All          => @"all",
			IssueScope.CreatedByMe  => @"created_by_me",
			IssueScope.AssignedToMe => @"assigned_to_me",
			_ => throw new ArgumentException($"Unknown scope: {scope}.", nameof(scope)),
		};

	public Task<IReadOnlyList<Issue>> GetProjectIssuesAsync(
		NameOrNumericId projectId,
		IssueState? state      = default,
		IssueScope? scope      = default,
		long?       assigneeId = default,
		long?       authorId   = default,
		SortOrder?  sort       = default,
		CancellationToken cancellationToken = default)
	{
		/*
		author_username
		confidential
		created_after
		created_before
		due_date
		epic_id
		iids[]
		in
		issue_type
		iteration_id
		iteration_title
		labels
		milestone
		milestone_id
		my_reaction_emoji
		non_archived
		not
		order_by
		search
		updated_after
		updated_before
		weight
		with_labels_details
		 */

		var query = new StringBuilder();
		AppendProjectUrl(query, projectId, @"issues");

		var sep = '?';

		if(assigneeId.HasValue) AppendParameter(query, ref sep, @"assignee_id", assigneeId.Value.ToString(CultureInfo.InvariantCulture));
		if(authorId.HasValue)   AppendParameter(query, ref sep, @"author_id",   authorId.Value.ToString(CultureInfo.InvariantCulture));
		if(state.HasValue)      AppendParameter(query, ref sep, @"state",       StateToString(state.Value));
		if(scope.HasValue)      AppendParameter(query, ref sep, @"scope",       ScopeToString(scope.Value));
		if(sort.HasValue)       AppendParameter(query, ref sep, @"sort",        SortToString(sort.Value));

		return ReadPagedResultAsync<Issue>(query.ToString(), cancellationToken);
	}
}
