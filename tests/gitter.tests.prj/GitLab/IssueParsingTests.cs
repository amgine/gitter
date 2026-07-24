#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System.Text.Json;

using NUnit.Framework;

[TestFixture]
class IssueParsingTests
{
	private const string WorkItemJson = """
		{
		  "id": 41,
		  "iid": 1,
		  "project_id": 4,
		  "title": "Something is broken",
		  "description": "steps to reproduce",
		  "state": "opened",
		  "type": "ISSUE",
		  "issue_type": "issue",
		  "severity": "UNKNOWN",
		  "health_status": "on_track",
		  "confidential": false,
		  "discussion_locked": null,
		  "merge_requests_count": 2,
		  "user_notes_count": 7,
		  "web_url": "https://gitlab.example.com/group/project/-/issues/1",
		  "created_at": "2026-01-04T15:31:51.081Z",
		  "updated_at": "2026-02-11T10:12:00.000Z",
		  "labels": ["bug", "ui"],
		  "references": {
		    "short": "#1",
		    "relative": "#1",
		    "full": "group/project#1"
		  },
		  "author": { "id": 3, "name": "Author", "username": "author", "state": "active", "avatar_url": "", "web_url": "" }
		}
		""";

	[Test]
	public void Parse_WorkItemPayload_PopulatesNewFields()
	{
		var issue = JsonSerializer.Deserialize<Issue>(WorkItemJson);

		Assert.That(issue,                Is.Not.Null);
		Assert.That(issue!.Id,            Is.EqualTo(41));
		Assert.That(issue.Iid,            Is.EqualTo(1));
		Assert.That(issue.IssueType,      Is.EqualTo(WorkItemType.Issue));
		Assert.That(issue.Type,           Is.EqualTo(WorkItemType.Issue));
		Assert.That(issue.EffectiveType,  Is.EqualTo(WorkItemType.Issue));
		Assert.That(issue.Severity,       Is.EqualTo("UNKNOWN"));
		Assert.That(issue.HealthStatus,   Is.EqualTo("on_track"));
		Assert.That(issue.MergeRequestsCount, Is.EqualTo(2));
		Assert.That(issue.DiscussionLocked,   Is.Null);
		Assert.That(issue.References,         Is.Not.Null);
		Assert.That(issue.References!.Full,   Is.EqualTo("group/project#1"));
		Assert.That(issue.References.ToString(), Is.EqualTo("group/project#1"));
	}

	[TestCase(@"issue",       WorkItemType.Issue)]
	[TestCase(@"incident",    WorkItemType.Incident)]
	[TestCase(@"test_case",   WorkItemType.TestCase)]
	[TestCase(@"task",        WorkItemType.Task)]
	[TestCase(@"epic",        WorkItemType.Epic)]
	[TestCase(@"objective",   WorkItemType.Objective)]
	[TestCase(@"key_result",  WorkItemType.KeyResult)]
	[TestCase(@"requirement", WorkItemType.Requirement)]
	[TestCase(@"ticket",      WorkItemType.Ticket)]
	public void Parse_LowerSnakeCaseIssueType(string wire, WorkItemType expected)
	{
		var issue = JsonSerializer.Deserialize<Issue>($$"""{ "issue_type": "{{wire}}" }""")!;
		Assert.That(issue.IssueType, Is.EqualTo(expected));
	}

	[TestCase(@"ISSUE",     WorkItemType.Issue)]
	[TestCase(@"TEST_CASE", WorkItemType.TestCase)]
	[TestCase(@"KEY_RESULT", WorkItemType.KeyResult)]
	public void Parse_UpperSnakeCaseType(string wire, WorkItemType expected)
	{
		var issue = JsonSerializer.Deserialize<Issue>($$"""{ "type": "{{wire}}" }""")!;
		Assert.That(issue.Type, Is.EqualTo(expected));
	}

	[Test]
	public void Parse_UnknownWorkItemType_DoesNotThrow()
	{
		var issue = JsonSerializer.Deserialize<Issue>("""{ "iid": 5, "issue_type": "brand_new_type" }""");

		Assert.That(issue,           Is.Not.Null);
		Assert.That(issue!.Iid,      Is.EqualTo(5));
		Assert.That(issue.IssueType, Is.EqualTo(WorkItemType.Unknown));
	}

	[Test]
	public void Parse_MissingType_IsUnknown()
	{
		var issue = JsonSerializer.Deserialize<Issue>("""{ "iid": 5 }""")!;
		Assert.That(issue.IssueType,     Is.EqualTo(WorkItemType.Unknown));
		Assert.That(issue.EffectiveType, Is.EqualTo(WorkItemType.Unknown));
	}

	[Test]
	public void Parse_NullType_IsUnknown()
	{
		var issue = JsonSerializer.Deserialize<Issue>("""{ "iid": 5, "issue_type": null }""")!;
		Assert.That(issue.IssueType, Is.EqualTo(WorkItemType.Unknown));
	}

	[Test]
	public void EffectiveType_FallsBackToTypeField()
	{
		var issue = JsonSerializer.Deserialize<Issue>("""{ "type": "INCIDENT" }""")!;
		Assert.That(issue.IssueType,     Is.EqualTo(WorkItemType.Unknown));
		Assert.That(issue.EffectiveType, Is.EqualTo(WorkItemType.Incident));
	}

	[TestCase(@"opened",   IssueState.Opened)]
	[TestCase(@"reopened", IssueState.Opened)]
	[TestCase(@"closed",   IssueState.Closed)]
	[TestCase(@"CLOSED",   IssueState.Closed)]
	public void Parse_KnownStates(string wire, IssueState expected)
	{
		var issue = JsonSerializer.Deserialize<Issue>($$"""{ "state": "{{wire}}" }""")!;
		Assert.That(issue.State, Is.EqualTo(expected));
	}

	[Test]
	public void Parse_UnknownState_DoesNotAbortTheWholePage()
	{
		var json = """[{ "iid": 1, "state": "some_future_state" }, { "iid": 2, "state": "opened" }]""";

		var issues = JsonSerializer.Deserialize<Issue[]>(json);

		Assert.That(issues,       Is.Not.Null);
		Assert.That(issues!,      Has.Length.EqualTo(2));
		Assert.That(issues[0].State, Is.EqualTo(IssueState.Unknown));
		Assert.That(issues[1].State, Is.EqualTo(IssueState.Opened));
	}

	[Test]
	public void Parse_UnknownMilestoneState_DoesNotThrow()
	{
		var issue = JsonSerializer.Deserialize<Issue>(
			"""{ "iid": 1, "milestone": { "id": 1, "title": "v1", "state": "brand_new" } }""")!;

		Assert.That(issue.Milestone,        Is.Not.Null);
		Assert.That(issue.Milestone!.Title, Is.EqualTo("v1"));
		Assert.That(issue.Milestone.State,  Is.EqualTo(MilestoneState.Unknown));
	}

	[Test]
	public void Parse_IdentifiersBeyondInt32_AreNotTruncated()
	{
		var issue = JsonSerializer.Deserialize<Issue>("""{ "id": 4294967296, "iid": 3, "project_id": 3000000000 }""")!;

		Assert.That(issue.Id,        Is.EqualTo(4294967296L));
		Assert.That(issue.ProjectId, Is.EqualTo(3000000000L));
	}
}
