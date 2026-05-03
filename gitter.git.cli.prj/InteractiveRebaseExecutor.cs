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

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>Drives <c>git rebase -i</c> via fake sequence editor + named pipe IPC.</summary>
internal sealed class InteractiveRebaseExecutor : IInteractiveRebaseExecutor
{
	static readonly string RebaseEditorPath = Path.Combine(
		AppContext.BaseDirectory, "gitter.rebase-editor.exe");

	readonly string         _workingDirectory;
	readonly CommandBuilder _commandBuilder;

	internal InteractiveRebaseExecutor(string workingDirectory, CommandBuilder commandBuilder)
	{
		_workingDirectory = workingDirectory;
		_commandBuilder   = commandBuilder;
	}

	/// <summary>Start <c>git rebase -i</c> and return a session once the fake editor hands off the todo file path.</summary>
	public async Task<IInteractiveRebaseSession> StartAsync(
		string            upstream,
		string?           onto              = null,
		CancellationToken cancellationToken = default)
	{
		if(!File.Exists(RebaseEditorPath))
			throw new FileNotFoundException(
				$"Interactive rebase helper not found at {RebaseEditorPath}. Please reinstall Gitter.",
				RebaseEditorPath);

		var gitExe = GitProcess.GitExePath;
		if(string.IsNullOrEmpty(gitExe) || !File.Exists(gitExe))
			throw new InvalidOperationException(
				"Git executable not found. Configure the path to git.exe in Gitter settings.");

		var pipeServer = new RebaseNamedPipeServer();
		var command    = _commandBuilder.GetInteractiveRebaseCommand(upstream, onto);
		var arguments  = command.ToString();

		var psi = new ProcessStartInfo(gitExe, arguments)
		{
			WorkingDirectory       = _workingDirectory,
			WindowStyle            = ProcessWindowStyle.Hidden,
			UseShellExecute        = false,
			RedirectStandardOutput = true,
			RedirectStandardError  = true,
			CreateNoWindow         = true,
			LoadUserProfile        = true,
		};
		GitProcess.SetCriticalEnvironmentVariables(psi);
		psi.EnvironmentVariables["GIT_SEQUENCE_EDITOR"] =
			$"\"{RebaseEditorPath}\" {pipeServer.PipeName}";

		var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
		process.Start();

		var stderr = new System.Text.StringBuilder();
		process.ErrorDataReceived += (_, args) =>
		{
			if(args.Data is not null) stderr.AppendLine(args.Data);
		};
		process.OutputDataReceived += (_, _) => { };
		process.BeginErrorReadLine();
		process.BeginOutputReadLine();

		// race pipe connection vs. process exit — exit first means editor failed to launch
		string todoFilePath;
		try
		{
			var pipeTask = pipeServer.WaitForTodoFilePathAsync(cancellationToken);
			var exitTask = process.WaitForExitAsync(cancellationToken);
			var winner   = await Task.WhenAny(pipeTask, exitTask).ConfigureAwait(false);

			if(winner == exitTask)
			{
				pipeServer.Dispose();
				var msg = stderr.Length > 0
					? stderr.ToString()
					: $"git rebase exited with code {process.ExitCode} before the editor could connect.";
				throw new InvalidOperationException(msg);
			}

			todoFilePath = await pipeTask.ConfigureAwait(false);
		}
		catch
		{
			try { if(!process.HasExited) process.Kill(); } catch { }
			pipeServer.Dispose();
			throw;
		}

		var entries = RebaseTodoParser.Read(todoFilePath);
		return new Session(entries, todoFilePath, process, pipeServer, stderr);
	}

	sealed class Session : IInteractiveRebaseSession
	{
		readonly List<RebaseTodoEntry>      _entries;
		readonly Process                    _process;
		readonly RebaseNamedPipeServer      _pipe;
		readonly System.Text.StringBuilder  _stderr;
		bool                                _completed;

		public Session(
			IReadOnlyList<RebaseTodoEntry> entries,
			string                         todoFilePath,
			Process                        process,
			RebaseNamedPipeServer          pipe,
			System.Text.StringBuilder      stderr)
		{
			_entries     = new List<RebaseTodoEntry>(entries);
			TodoFilePath = todoFilePath;
			_process     = process;
			_pipe        = pipe;
			_stderr      = stderr;
		}

		public IReadOnlyList<RebaseTodoEntry> Entries      => _entries;
		public string                         TodoFilePath { get; }

		public async Task<RebaseResult> CommitPlanAsync(
			IReadOnlyList<RebaseTodoEntry> modified,
			CancellationToken              cancellationToken = default)
		{
			if(_completed) throw new InvalidOperationException("Session already completed.");
			_completed = true;

			RebaseTodoParser.Write(TodoFilePath, modified);
			await _pipe.SendDoneSignalAsync(cancellationToken).ConfigureAwait(false);
			_pipe.Dispose();

			await _process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
			return new RebaseResult(_process.ExitCode, _stderr.ToString());
		}

		public Task AbortAsync(CancellationToken cancellationToken = default)
		{
			if(_completed) return Task.CompletedTask;
			_completed = true;

			_pipe.Dispose();
			try { _process.Kill(); } catch { }
			return Task.CompletedTask;
		}
	}
}
