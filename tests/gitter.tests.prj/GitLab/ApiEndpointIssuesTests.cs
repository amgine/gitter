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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

[TestFixture]
class ApiEndpointIssuesTests
{
	private sealed class RecordingHandler : HttpMessageHandler
	{
		private readonly Queue<HttpResponseMessage> _responses;
		public List<string> RequestedUrls { get; } = new();
		public RecordingHandler(params HttpResponseMessage[] responses)
		{
			_responses = new Queue<HttpResponseMessage>(responses);
		}
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
		{
			RequestedUrls.Add(request.RequestUri!.AbsoluteUri);
			var response = _responses.Count > 0 ? _responses.Dequeue() : new HttpResponseMessage(HttpStatusCode.NotFound);
			response.RequestMessage = request;
			return Task.FromResult(response);
		}
	}

	private static ApiEndpoint MakeEndpoint(RecordingHandler handler)
	{
		var invoker = new HttpMessageInvoker(handler);
		var server  = new ServerInfo("test", new Uri("https://gitlab.example.com"), "token");
		return new ApiEndpoint(invoker, server);
	}

	private static HttpResponseMessage Json(string body, HttpStatusCode status = HttpStatusCode.OK)
		=> new(status) { Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json") };

	private static async Task<string> RequestUrlAsync(Func<ApiEndpoint, Task> call)
	{
		var handler  = new RecordingHandler(Json("[]"));
		var endpoint = MakeEndpoint(handler);
		await call(endpoint);
		Assert.That(handler.RequestedUrls, Has.Count.EqualTo(1));
		return handler.RequestedUrls[0];
	}

	#region project path

	[Test]
	public async Task ProjectPath_IsUrlEncoded()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromName("group/project")));
		Assert.That(url, Does.Contain("/api/v4/projects/group%2Fproject/issues"));
	}

	[Test]
	public async Task NestedProjectPath_EncodesSlashesButNotDots()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(
			NameOrNumericId.FromName("promcontrol/viscont/nugets/viscont.core.framework.sparkpulse")));

		Assert.That(url, Does.Contain(
			"/api/v4/projects/promcontrol%2Fviscont%2Fnugets%2Fviscont.core.framework.sparkpulse/issues"));
	}

	[Test]
	public async Task NumericProjectId_IsUsedVerbatim()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(42)));
		Assert.That(url, Does.Contain("/api/v4/projects/42/issues"));
	}

	#endregion

	#region work item type filter

	[TestCase(WorkItemType.Issue,    @"issue_type=issue")]
	[TestCase(WorkItemType.Incident, @"issue_type=incident")]
	[TestCase(WorkItemType.TestCase, @"issue_type=test_case")]
	[TestCase(WorkItemType.Task,     @"issue_type=task")]
	public async Task IssueType_IsSentAsSnakeCase(WorkItemType type, string expected)
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1), issueType: type));
		Assert.That(url, Does.Contain(expected));
	}

	[Test]
	public async Task IssueType_OmittedWhenNotRequested()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1)));
		Assert.That(url, Does.Not.Contain("issue_type"));
	}

	[Test]
	public void IssueType_Unknown_IsRejected()
	{
		var handler  = new RecordingHandler(Json("[]"));
		var endpoint = MakeEndpoint(handler);

		Assert.ThrowsAsync<ArgumentException>(
			() => endpoint.GetProjectIssuesAsync(NameOrNumericId.FromId(1), issueType: WorkItemType.Unknown));
	}

	#endregion

	#region filters

	[Test]
	public async Task State_And_Scope_AreSent()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			state: IssueState.Closed, scope: IssueScope.AssignedToMe));

		Assert.That(url, Does.Contain("state=closed"));
		Assert.That(url, Does.Contain("scope=assigned_to_me"));
	}

	[Test]
	public async Task OrderBy_IsSentAsSnakeCase()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			orderBy: IssueOrderBy.UpdatedAt, sort: SortOrder.Ascending));

		Assert.That(url, Does.Contain("order_by=updated_at"));
		Assert.That(url, Does.Contain("sort=asc"));
	}

	[Test]
	public async Task Labels_AreCommaSeparatedAndIndividuallyEncoded()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			labels: ["needs review", "ui"]));

		Assert.That(url, Does.Contain("labels=needs%20review,ui"));
	}

	[Test]
	public async Task Labels_NoneKeyword_IsNotEncoded()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			labels: [ApiEndpoint.NoneFilter]));

		Assert.That(url, Does.Contain("labels=None"));
	}

	[Test]
	public async Task Milestone_AnyKeyword_IsNotEncoded()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			milestone: ApiEndpoint.AnyFilter));

		Assert.That(url, Does.Contain("milestone=Any"));
	}

	[Test]
	public async Task Milestone_IsEncoded()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			milestone: "v1.0 rc"));

		Assert.That(url, Does.Contain("milestone=v1.0%20rc"));
	}

	[Test]
	public async Task Search_IsEncoded()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			search: "crash on start"));

		Assert.That(url, Does.Contain("search=crash%20on%20start"));
	}

	[Test]
	public async Task Iids_AreRepeated()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			iids: [3L, 4L]));

		Assert.That(url, Does.Contain("iids[]=3").IgnoreCase.Or.Contain("iids%5B%5D=3"));
		Assert.That(url, Does.Contain("4"));
	}

	[Test]
	public async Task Dates_AreEncodedSoPlusSurvives()
	{
		var stamp = new DateTimeOffset(2026, 3, 1, 12, 0, 0, TimeSpan.FromHours(3));
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			updatedAfter: stamp));

		Assert.That(url, Does.Not.Contain("+"));
		Assert.That(url, Does.Contain("updated_after=2026-03-01T12%3A00%3A00%2B0300"));
	}

	[Test]
	public async Task Confidential_IsSentAsBoolean()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			confidential: false));

		Assert.That(url, Does.Contain("confidential=false"));
	}

	#endregion

	#region pagination

	[Test]
	public async Task PageSize_IsRequestedExplicitly()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1)));
		Assert.That(url, Does.Contain("per_page=100"));
	}

	[Test]
	public async Task PageSize_IsAppendedToAnExistingQuery()
	{
		var url = await RequestUrlAsync(e => e.GetProjectIssuesAsync(NameOrNumericId.FromId(1),
			state: IssueState.Opened));

		Assert.That(url, Does.Contain("state=opened"));
		Assert.That(url, Does.Contain("per_page=100"));
		Assert.That(url.Split('?'), Has.Length.EqualTo(2));
	}

	[Test]
	public async Task LinkHeader_WithCommasInsideUrl_IsParsedCorrectly()
	{
		var page1 = Json("""[{"iid":1}]""");
		page1.Headers.Add("Link",
			"""<https://gitlab.example.com/api/v4/projects/1/issues?labels=bug,ui&page=2>; rel="next", <https://gitlab.example.com/api/v4/projects/1/issues?labels=bug,ui&page=9>; rel="last" """);
		var page2 = Json("""[{"iid":2}]""");

		var handler  = new RecordingHandler(page1, page2);
		var endpoint = MakeEndpoint(handler);

		var issues = await endpoint.GetProjectIssuesAsync(NameOrNumericId.FromId(1), labels: ["bug", "ui"]);

		Assert.That(handler.RequestedUrls, Has.Count.EqualTo(2));
		Assert.That(handler.RequestedUrls[1], Does.Contain("page=2"));
		Assert.That(issues, Has.Count.EqualTo(2));
	}

	[Test]
	public async Task LinkHeader_WithoutNextRelation_StopsAfterOnePage()
	{
		var page1 = Json("""[{"iid":1}]""");
		page1.Headers.Add("Link",
			"""<https://gitlab.example.com/api/v4/projects/1/issues?page=1>; rel="prev" """);

		var handler  = new RecordingHandler(page1);
		var endpoint = MakeEndpoint(handler);

		var issues = await endpoint.GetProjectIssuesAsync(NameOrNumericId.FromId(1));

		Assert.That(handler.RequestedUrls, Has.Count.EqualTo(1));
		Assert.That(issues, Has.Count.EqualTo(1));
	}

	#endregion

	#region error reporting

	[Test]
	public void NotFound_ThrowsWithServerMessageAndUrl()
	{
		var handler  = new RecordingHandler(Json("""{"message":"404 Project Not Found"}""", HttpStatusCode.NotFound));
		var endpoint = MakeEndpoint(handler);

		var ex = Assert.ThrowsAsync<GitLabApiException>(
			() => endpoint.GetProjectIssuesAsync(NameOrNumericId.FromName("group/project.git")));

		Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
		Assert.That(ex.Details,     Is.EqualTo("404 Project Not Found"));
		Assert.That(ex.Message,     Does.Contain("404 Project Not Found"));
		Assert.That(ex.Message,     Does.Contain("group%2Fproject.git"));
	}

	[Test]
	public void Unauthorized_StillThrowsUnauthorizedAccessException()
	{
		var handler  = new RecordingHandler(Json("""{"message":"401 Unauthorized"}""", HttpStatusCode.Unauthorized));
		var endpoint = MakeEndpoint(handler);

		var ex = Assert.ThrowsAsync<GitLabUnauthorizedException>(
			() => endpoint.GetProjectIssuesAsync(NameOrNumericId.FromId(1)));

		Assert.That(ex, Is.InstanceOf<UnauthorizedAccessException>());
		Assert.That(ex!.Details, Is.EqualTo("401 Unauthorized"));
	}

	[Test]
	public void NonJsonErrorBody_IsStillReported()
	{
		var handler = new RecordingHandler(
			new HttpResponseMessage(HttpStatusCode.BadGateway) { Content = new StringContent("<html>bad gateway</html>") });
		var endpoint = MakeEndpoint(handler);

		var ex = Assert.ThrowsAsync<GitLabApiException>(
			() => endpoint.GetProjectIssuesAsync(NameOrNumericId.FromId(1)));

		Assert.That(ex!.StatusCode, Is.EqualTo(HttpStatusCode.BadGateway));
		Assert.That(ex.Details,     Does.Contain("bad gateway"));
	}

	#endregion
}
