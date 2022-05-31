﻿#region Copyright Notice
/*
* gitter - VCS repository management tool
* Copyright (C) 2019  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.CLI;

abstract class CommandExecutorBase : ICommandExecutor
{
	#region .ctor

	protected CommandExecutorBase(ICliOptionsProvider cliOptionsProvider)
	{
		Verify.Argument.IsNotNull(cliOptionsProvider);

		CliOptionsProvider = cliOptionsProvider;
	}

	#endregion

	#region Properties

	protected ICliOptionsProvider CliOptionsProvider { get; }

	#endregion

	#region Methods

	protected abstract GitInput PrepareInput(Command command, Encoding encoding);

	protected virtual ProcessExecutor<GitInput> CreateProcessExecutor()
		=> new GitProcessExecutor(CliOptionsProvider.GitExecutablePath);

	protected virtual void OnCommandExecuting(Command command)
	{
	}

	private static Action<Process> GetCancellationMethod(CommandExecutionFlags flags)
		=> ProcessExecutor.CancellationMethods.AllowToExecute;
		//=> (flags & CommandExecutionFlags.DoNotKillProcess) == CommandExecutionFlags.DoNotKillProcess
		//	? ProcessExecutor.CancellationMethods.AllowToExecute
		//	: ProcessExecutor.CancellationMethods.KillProcess;

	public GitOutput ExecuteCommand(Command command, CommandExecutionFlags flags)
	{
		OnCommandExecuting(command);

		var stdOutReceiver = new AsyncTextReader();
		var stdErrReceiver = new AsyncTextReader();
		var executor = CreateProcessExecutor();
		var exitCode = executor.Execute(PrepareInput(command, GitProcess.DefaultEncoding), stdOutReceiver, stdErrReceiver);
		return new GitOutput(stdOutReceiver.GetText(), stdErrReceiver.GetText(), exitCode);
	}

	public GitOutput ExecuteCommand(Command command, Encoding encoding, CommandExecutionFlags flags)
	{
		OnCommandExecuting(command);

		var stdOutReceiver = new AsyncTextReader();
		var stdErrReceiver = new AsyncTextReader();
		var executor = CreateProcessExecutor();
		var exitCode = executor.Execute(PrepareInput(command, encoding), stdOutReceiver, stdErrReceiver);
		return new GitOutput(stdOutReceiver.GetText(), stdErrReceiver.GetText(), exitCode);
	}

	public int ExecuteCommand(Command command, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CommandExecutionFlags flags)
	{
		OnCommandExecuting(command);

		var executor = CreateProcessExecutor();
		var exitCode = executor.Execute(PrepareInput(command, GitProcess.DefaultEncoding), stdOutReceiver, stdErrReceiver);
		return exitCode;
	}

	public int ExecuteCommand(Command command, Encoding encoding, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CommandExecutionFlags flags)
	{
		OnCommandExecuting(command);

		var executor = CreateProcessExecutor();
		var exitCode = executor.Execute(PrepareInput(command, encoding), stdOutReceiver, stdErrReceiver);
		return exitCode;
	}

	private Task<int> ExecuteCommandAsyncCore(Command command, Encoding encoding, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CommandExecutionFlags flags, CancellationToken cancellationToken = default)
	{
		OnCommandExecuting(command);

		var input              = PrepareInput(command, encoding);
		var processExecutor    = CreateProcessExecutor();
		var cancellationMethod = GetCancellationMethod(flags);
		return processExecutor.ExecuteAsync(
			input,
			stdOutReceiver,
			stdErrReceiver,
			cancellationMethod,
			cancellationToken);
	}

	private async Task<GitOutput> ExecuteCommandAsyncCore(Command command, Encoding encoding, CommandExecutionFlags flags, CancellationToken cancellationToken = default)
	{
		Assert.IsNotNull(command);

		OnCommandExecuting(command);

		var stdOutReceiver     = new AsyncTextReader();
		var stdErrReceiver     = new AsyncTextReader();
		var input              = PrepareInput(command, encoding);
		var processExecutor    = CreateProcessExecutor();
		var cancellationMethod = GetCancellationMethod(flags);
		var exitCode = await processExecutor
			.ExecuteAsync(
				input,
				stdOutReceiver,
				stdErrReceiver,
				cancellationMethod,
				cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var stdOut = stdOutReceiver.GetText();
		var stdErr = stdErrReceiver.GetText();
		return new GitOutput(stdOut, stdErr, exitCode);
	}

	public Task<GitOutput> ExecuteCommandAsync(Command command, CommandExecutionFlags flags, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(command);

		return ExecuteCommandAsyncCore(command, GitProcess.DefaultEncoding, flags, cancellationToken);
	}

	public Task<GitOutput> ExecuteCommandAsync(Command command, Encoding encoding, CommandExecutionFlags flags, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(command);

		return ExecuteCommandAsyncCore(command, encoding, flags, cancellationToken);
	}

	public Task<int> ExecuteCommandAsync(Command command, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CommandExecutionFlags flags, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(command);

		return ExecuteCommandAsyncCore(command, GitProcess.DefaultEncoding, stdOutReceiver, stdErrReceiver, flags, cancellationToken);
	}

	public Task<int> ExecuteCommandAsync(Command command, Encoding encoding, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver, CommandExecutionFlags flags, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(command);

		return ExecuteCommandAsyncCore(command, encoding, stdOutReceiver, stdErrReceiver, flags, cancellationToken);
	}

	#endregion
}
