#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>Named pipe server for IPC with gitter.rebase-editor.exe.</summary>
internal sealed class RebaseNamedPipeServer : IDisposable
{
	static readonly byte[] DoneSignal = [1];

	readonly NamedPipeServerStream _pipe;

	public RebaseNamedPipeServer()
	{
		PipeName = "gitter-rebase-" + Guid.NewGuid().ToString("N");
		_pipe    = new NamedPipeServerStream(
			PipeName,
			PipeDirection.InOut,
			maxNumberOfServerInstances: 1,
			transmissionMode: PipeTransmissionMode.Byte,
			options: PipeOptions.Asynchronous);
	}

	public string PipeName { get; }

	/// <summary>Wait for the fake editor to connect and return the todo file path it sends.</summary>
	public async Task<string> WaitForTodoFilePathAsync(CancellationToken cancellationToken = default)
	{
		await _pipe.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);

		using var reader = new StreamReader(_pipe, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: -1, leaveOpen: true);
		var path = await reader
#if NET7_0_OR_GREATER
			.ReadLineAsync(cancellationToken)
#else
			.ReadLineAsync()
#endif
			.ConfigureAwait(false);
		return path ?? string.Empty;
	}

	/// <summary>Send done signal so the fake editor exits and git resumes.</summary>
	public async Task SendDoneSignalAsync(CancellationToken cancellationToken = default)
	{
#if NETCOREAPP
		var signal = new ReadOnlyMemory<byte>(DoneSignal);
		await _pipe.WriteAsync(signal, cancellationToken).ConfigureAwait(false);
#else
		await _pipe.WriteAsync(DoneSignal, 0, DoneSignal.Length, cancellationToken).ConfigureAwait(false);
#endif
		await _pipe.FlushAsync(cancellationToken).ConfigureAwait(false);
	}

	public void Dispose() => _pipe.Dispose();
}
