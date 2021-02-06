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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.CLI;

	using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

	sealed class PushImpl : IGitFunction<PushParameters, IList<ReferencePushResult>>
	{
		private readonly ICommandExecutor _commandExecutor;
		private readonly Func<PushParameters, bool, Command> _commandFactory;
		private readonly Func<string, IList<ReferencePushResult>> _resultsParser;

		public PushImpl(
			ICommandExecutor commandExecutor,
			Func<PushParameters, bool, Command> commandFactory,
			Func<string, IList<ReferencePushResult>> resultsParser)
		{
			_commandExecutor = commandExecutor;
			_commandFactory  = commandFactory;
			_resultsParser   = resultsParser;
		}

		public IList<ReferencePushResult> Invoke(PushParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, nameof(parameters));

			var command = _commandFactory(parameters, false);
			var output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();
			return _resultsParser(output.Output);
		}

		public async Task<IList<ReferencePushResult>> InvokeAsync(PushParameters parameters,
			IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			Verify.Argument.IsNotNull(parameters, nameof(parameters));

			var command = _commandFactory(parameters, true);

			progress?.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));
			var errorMessages  = default(List<string>);
			var stdOutReceiver = new AsyncTextReader();
			var stdErrReceiver = new NotifyingAsyncTextReader();
			stdErrReceiver.TextLineReceived += (s, e) =>
			{
				if(!string.IsNullOrWhiteSpace(e.Text))
				{
					var parser = new GitParser(e.Text);
					var operationProgress = parser.ParseProgress();
					progress?.Report(operationProgress);
					if(operationProgress.IsIndeterminate)
					{
						errorMessages ??= new List<string>();
						errorMessages.Add(operationProgress.ActionName);
					}
					else
					{
						errorMessages?.Clear();
					}
				}
			};

			var processExitCode = await _commandExecutor
				.ExecuteCommandAsync(
					command,
					stdOutReceiver,
					stdErrReceiver,
					CommandExecutionFlags.None,
					cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			if(processExitCode != 0)
			{
				var errorMessage = errorMessages != null && errorMessages.Count != 0
					? string.Join(Environment.NewLine, errorMessages)
					: string.Format(CultureInfo.InvariantCulture, "git process exited with code {0}", processExitCode);
				throw new GitException(errorMessage);
			}
			return _resultsParser(stdOutReceiver.GetText());
		}
	}
}
