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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

partial class ApiEndpoint
{
	public const string NoneFilter = @"None";

	public const string AnyFilter = @"Any";

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

	private static string OrderToString(IssueOrderBy order)
		=> order switch
		{
			IssueOrderBy.CreatedAt        => @"created_at",
			IssueOrderBy.UpdatedAt        => @"updated_at",
			IssueOrderBy.Priority         => @"priority",
			IssueOrderBy.DueDate          => @"due_date",
			IssueOrderBy.RelativePosition => @"relative_position",
			IssueOrderBy.LabelPriority    => @"label_priority",
			IssueOrderBy.MilestoneDue     => @"milestone_due",
			IssueOrderBy.Popularity       => @"popularity",
			IssueOrderBy.Weight           => @"weight",
			IssueOrderBy.Title            => @"title",
			_ => throw new ArgumentException($"Unknown order: {order}.", nameof(order)),
		};

	private static string IssueTypeToString(WorkItemType type)
		=> WorkItemTypes.ToApiString(type)
		?? throw new ArgumentException($"Unknown work item type: {type}.", nameof(type));

	private static string EncodeFilterValue(string value)
		=> value is NoneFilter or AnyFilter ? value : Uri.EscapeDataString(value);

	public Task<IReadOnlyList<Issue>> GetProjectIssuesAsync(
		NameOrNumericId projectId,
		IssueState?     state         = default,
		IssueScope?     scope         = default,
		WorkItemType?   issueType     = default,
		long?           assigneeId    = default,
		long?           authorId      = default,
		IssueOrderBy?   orderBy       = default,
		SortOrder?      sort          = default,
		IReadOnlyCollection<string>? labels = default,
		string?         milestone     = default,
		string?         search        = default,
		IReadOnlyCollection<long>? iids = default,
		DateTimeOffset? createdAfter  = default,
		DateTimeOffset? createdBefore = default,
		DateTimeOffset? updatedAfter  = default,
		DateTimeOffset? updatedBefore = default,
		bool?           confidential  = default,
		bool            withLabelsDetails = false,
		CancellationToken cancellationToken = default)
	{
		var query = new StringBuilder();
		AppendProjectUrl(query, projectId, @"issues");

		var sep = '?';

		if(assigneeId.HasValue) AppendParameter(query, ref sep, @"assignee_id", assigneeId.Value.ToString(CultureInfo.InvariantCulture));
		if(authorId.HasValue)   AppendParameter(query, ref sep, @"author_id",   authorId.Value.ToString(CultureInfo.InvariantCulture));
		if(state.HasValue)      AppendParameter(query, ref sep, @"state",       StateToString(state.Value));
		if(scope.HasValue)      AppendParameter(query, ref sep, @"scope",       ScopeToString(scope.Value));
		if(issueType.HasValue)  AppendParameter(query, ref sep, @"issue_type",  IssueTypeToString(issueType.Value));
		if(orderBy.HasValue)    AppendParameter(query, ref sep, @"order_by",    OrderToString(orderBy.Value));
		if(sort.HasValue)       AppendParameter(query, ref sep, @"sort",        SortToString(sort.Value));

		if(labels is { Count: > 0 })
		{
			AppendParameter(query, ref sep, @"labels",
				string.Join(",", labels.Select(EncodeFilterValue)));
		}
		if(!string.IsNullOrEmpty(milestone))
		{
			AppendParameter(query, ref sep, @"milestone", EncodeFilterValue(milestone!));
		}
		if(!string.IsNullOrEmpty(search))
		{
			AppendParameter(query, ref sep, @"search", Uri.EscapeDataString(search!));
		}
		if(iids is { Count: > 0 })
		{
			foreach(var iid in iids)
			{
				AppendParameter(query, ref sep, @"iids[]", iid.ToString(CultureInfo.InvariantCulture));
			}
		}

		if(createdAfter.HasValue)  AppendParameter(query, ref sep, @"created_after",  Uri.EscapeDataString(DateTimeHelper.FormatISO8601(createdAfter.Value)));
		if(createdBefore.HasValue) AppendParameter(query, ref sep, @"created_before", Uri.EscapeDataString(DateTimeHelper.FormatISO8601(createdBefore.Value)));
		if(updatedAfter.HasValue)  AppendParameter(query, ref sep, @"updated_after",  Uri.EscapeDataString(DateTimeHelper.FormatISO8601(updatedAfter.Value)));
		if(updatedBefore.HasValue) AppendParameter(query, ref sep, @"updated_before", Uri.EscapeDataString(DateTimeHelper.FormatISO8601(updatedBefore.Value)));

		if(confidential.HasValue)  AppendParameter(query, ref sep, @"confidential", confidential.Value ? @"true" : @"false");
		if(withLabelsDetails)      AppendParameter(query, ref sep, @"with_labels_details", @"true");

		return ReadPagedResultAsync<Issue>(query.ToString(), cancellationToken);
	}
}
