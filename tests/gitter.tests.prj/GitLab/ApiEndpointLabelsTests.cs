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
class ApiEndpointLabelsTests
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
			RequestedUrls.Add(request.RequestUri!.ToString());
			return Task.FromResult(_responses.Count > 0 ? _responses.Dequeue() : new HttpResponseMessage(HttpStatusCode.NotFound));
		}
	}

	private static ApiEndpoint MakeEndpoint(RecordingHandler handler)
	{
		var invoker = new HttpMessageInvoker(handler);
		var server  = new ServerInfo("test", new Uri("https://gitlab.example.com"), "token");
		return new ApiEndpoint(invoker, server);
	}

	private static HttpResponseMessage Json(string body)
		=> new(HttpStatusCode.OK) { Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json") };

	[Test]
	public async Task GetProjectLabels_BuildsCorrectUrl_WithCounts()
	{
		var handler = new RecordingHandler(Json("[]"));
		var endpoint = MakeEndpoint(handler);

		await endpoint.GetProjectLabelsAsync(NameOrNumericId.FromId(42), withCounts: true);

		Assert.That(handler.RequestedUrls, Has.Count.EqualTo(1));
		var url = handler.RequestedUrls[0];
		Assert.That(url, Does.Contain("/api/v4/projects/42/labels"));
		Assert.That(url, Does.Contain("with_counts=true"));
	}

	[Test]
	public async Task GetProjectLabels_WithoutCounts_OmitsParam()
	{
		var handler = new RecordingHandler(Json("[]"));
		var endpoint = MakeEndpoint(handler);

		await endpoint.GetProjectLabelsAsync(NameOrNumericId.FromId(7));

		Assert.That(handler.RequestedUrls[0], Does.Not.Contain("with_counts"));
	}

	[Test]
	public async Task GetProjectLabels_FollowsLinkHeaderAcrossTwoPages()
	{
		var page1 = Json("[{\"id\":1,\"name\":\"a\",\"color\":\"#000\"}]");
		page1.Headers.Add("Link", "<https://gitlab.example.com/api/v4/projects/1/labels?page=2>; rel=\"next\"");
		var page2 = Json("[{\"id\":2,\"name\":\"b\",\"color\":\"#fff\"}]");

		var handler  = new RecordingHandler(page1, page2);
		var endpoint = MakeEndpoint(handler);

		var labels = await endpoint.GetProjectLabelsAsync(NameOrNumericId.FromId(1));

		Assert.That(handler.RequestedUrls,        Has.Count.EqualTo(2));
		Assert.That(handler.RequestedUrls[1],     Does.Contain("page=2"));
		Assert.That(labels,                       Has.Count.EqualTo(2));
		Assert.That(labels[0].Name,               Is.EqualTo("a"));
		Assert.That(labels[1].Name,               Is.EqualTo("b"));
	}

	[Test]
	public void Cancellation_PropagatesThroughEndpoint()
	{
		var handler = new RecordingHandler(); // empty queue → 404
		var endpoint = MakeEndpoint(handler);
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		Assert.ThrowsAsync<OperationCanceledException>(
			() => endpoint.GetProjectLabelsAsync(NameOrNumericId.FromId(1), withCounts: false, cts.Token));
	}
}
