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
		#region Data

		private readonly ICommandExecutor _commandExecutor;
		private readonly Func<PushParameters, bool, Command> _commandFactory;
		private readonly Func<string, IList<ReferencePushResult>> _resultsParser;

		#endregion

		#region .ctor

		public PushImpl(
			ICommandExecutor commandExecutor,
			Func<PushParameters, bool, Command> commandFactory,
			Func<string, IList<ReferencePushResult>> resultsParser)
		{
			_commandExecutor = commandExecutor;
			_commandFactory = commandFactory;
			_resultsParser = resultsParser;
		}

		#endregion

		#region Methods

		public IList<ReferencePushResult> Invoke(PushParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = _commandFactory(parameters, false);
			var output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();
			return _resultsParser(output.Output);
		}

		public Task<IList<ReferencePushResult>> InvokeAsync(PushParameters parameters,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = _commandFactory(parameters, true);

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));
			}
			List<string> errorMessages = null;
			var stdOutReceiver = new AsyncTextReader();
			var stdErrReceiver = new NotifyingAsyncTextReader();
			stdErrReceiver.TextLineReceived += (s, e) =>
			{
				if(!string.IsNullOrWhiteSpace(e.Text))
				{
					var parser = new GitParser(e.Text);
					var operationProgress = parser.ParseProgress();
					if(progress != null)
					{
						progress.Report(operationProgress);
					}
					if(operationProgress.IsIndeterminate)
					{
						if(errorMessages == null)
						{
							errorMessages = new List<string>();
						}
						errorMessages.Add(operationProgress.ActionName);
					}
					else
					{
						if(errorMessages != null)
						{
							errorMessages.Clear();
						}
					}
				}
			};
			return _commandExecutor
				.ExecuteCommandAsync(
					command,
					stdOutReceiver,
					stdErrReceiver,
					CommandExecutionFlags.None,
					cancellationToken)
				.ContinueWith(task =>
				{
					int exitCode = TaskUtility.UnwrapResult(task);
					if(exitCode != 0)
					{
						string errorMessage;
						if(errorMessages != null && errorMessages.Count != 0)
						{
							errorMessage = string.Join(Environment.NewLine, errorMessages);
						}
						else
						{
							errorMessage = string.Format(CultureInfo.InvariantCulture, "git process exited with code {0}", exitCode);
						}
						throw new GitException(errorMessage);
					}
					else
					{
						return _resultsParser(stdOutReceiver.GetText());
					}
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		#endregion
	}
}
