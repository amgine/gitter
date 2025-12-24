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
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.CLI;

using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

sealed class PushFunction(
	ICommandExecutor                        commandExecutor,
	Func<PushRequest, bool, Command>        commandFactory,
	Func<string, Many<ReferencePushResult>> resultsParser)
	: IGitFunction<PushRequest, Many<ReferencePushResult>>
{
	public Many<ReferencePushResult> Invoke(PushRequest request)
	{
		Verify.Argument.IsNotNull(request);

		var command = commandFactory(request, false);
		var output  = commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
		output.ThrowOnBadReturnCode();
		return resultsParser(output.Output);
	}

	public async Task<Many<ReferencePushResult>> InvokeAsync(
		PushRequest                   request,
		IProgress<OperationProgress>? progress          = default,
		CancellationToken             cancellationToken = default)
	{
		Verify.Argument.IsNotNull(request);

		var command = commandFactory(request, true);

		progress?.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));
		var errorMessages  = default(List<string>);
		var stdOutReceiver = new AsyncTextReader();
		var stdErrReceiver = new NotifyingAsyncTextReader();
		stdErrReceiver.TextLineReceived += (_, e) =>
		{
			if(string.IsNullOrWhiteSpace(e.Text)) return;

			var parser = new GitParser(e.Text);
			var operationProgress = parser.ParseProgress();
			progress?.Report(operationProgress);
			if(operationProgress.IsIndeterminate)
			{
				errorMessages ??= [];
				errorMessages.Add(operationProgress.ActionName);
			}
			else
			{
				errorMessages?.Clear();
			}
		};

		var processExitCode = await commandExecutor
			.ExecuteCommandAsync(
				command,
				stdOutReceiver,
				stdErrReceiver,
				CommandExecutionFlags.None,
				cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		if(processExitCode != 0)
		{
			var errorMessage = errorMessages is { Count: not 0 }
				? string.Join(Environment.NewLine, errorMessages)
				: string.Format(CultureInfo.InvariantCulture, "git process exited with code {0}", processExitCode);
			throw new GitException(errorMessage);
		}
		return resultsParser(stdOutReceiver.GetText());
	}
}
