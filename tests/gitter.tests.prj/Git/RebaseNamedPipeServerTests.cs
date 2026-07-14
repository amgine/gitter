#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git;

using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using gitter.Git.AccessLayer.CLI;

using NUnit.Framework;

/// <summary>
/// Verifies the IPC contract between gitter.exe (server) and gitter.rebase-editor.exe (client).
/// Uses real named pipes — the client side here mimics what the rebase-editor does.
/// </summary>
[TestFixture]
class RebaseNamedPipeServerTests
{
	[Test, Timeout(10_000)]
	public async Task PipeName_IsUnique_AcrossInstances()
	{
		using var s1 = new RebaseNamedPipeServer();
		using var s2 = new RebaseNamedPipeServer();
		Assert.That(s1.PipeName, Is.Not.EqualTo(s2.PipeName));
		await Task.CompletedTask;
	}

	[Test, Timeout(10_000)]
	public async Task RoundTrip_ClientSendsPath_ServerReceivesIt()
	{
		using var server = new RebaseNamedPipeServer();
		const string expectedPath = @"C:\some\path\git-rebase-todo";

		// Server task
		var serverTask = server.WaitForTodoFilePathAsync(CancellationToken.None);

		// Client task — mimics gitter.rebase-editor.exe
		var clientTask = Task.Run(async () =>
		{
			using var pipe = new NamedPipeClientStream(".", server.PipeName, PipeDirection.InOut);
			await pipe.ConnectAsync(5_000);
			using var writer = new StreamWriter(pipe, Encoding.UTF8, leaveOpen: true);
			await writer.WriteLineAsync(expectedPath);
			await writer.FlushAsync();
			// Wait for done signal
			pipe.ReadByte();
		});

		var receivedPath = await serverTask;
		Assert.That(receivedPath, Is.EqualTo(expectedPath));

		await server.SendDoneSignalAsync();
		await clientTask;
	}

	[Test, Timeout(10_000)]
	public async Task SendDoneSignal_UnblocksClient()
	{
		using var server = new RebaseNamedPipeServer();

		var clientFinished = false;
		var clientTask = Task.Run(async () =>
		{
			using var pipe = new NamedPipeClientStream(".", server.PipeName, PipeDirection.InOut);
			await pipe.ConnectAsync(5_000);
			using var writer = new StreamWriter(pipe, Encoding.UTF8, leaveOpen: true);
			await writer.WriteLineAsync("/tmp/todo");
			await writer.FlushAsync();
			pipe.ReadByte(); // blocks until server sends done
			clientFinished = true;
		});

		await server.WaitForTodoFilePathAsync();
		Assert.That(clientFinished, Is.False, "client should still be blocked before done signal");

		await server.SendDoneSignalAsync();
		await clientTask;
		Assert.That(clientFinished, Is.True);
	}

	[Test, Timeout(10_000)]
	public void WaitForTodoFilePath_RespectsCancellation()
	{
		using var server = new RebaseNamedPipeServer();
		using var cts    = new CancellationTokenSource(millisecondsDelay: 200);

		Assert.ThrowsAsync<OperationCanceledException>(
			async () => await server.WaitForTodoFilePathAsync(cts.Token));
	}
}
