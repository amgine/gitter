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

	sealed class FetchOrPullImpl<TParameters> : IGitAction<TParameters>
		where TParameters : FetchParameters
	{
		private readonly ICommandExecutor _commandExecutor;
		private readonly Func<TParameters, bool, Command> _commandFactory;

		public FetchOrPullImpl(ICommandExecutor commandExecutor, Func<TParameters, bool, Command> commandFactory)
		{
			_commandExecutor = commandExecutor;
			_commandFactory = commandFactory;
		}

		public void Invoke(TParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = _commandFactory(parameters, false);
			var output = _commandExecutor.ExecuteCommand(command, null, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();
		}

		public Task InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));
			}
			List<string> errorMessages = null;
			var stdOutReceiver = new NullReader();
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
			var command = _commandFactory(parameters, true);
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
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}
	}
}
