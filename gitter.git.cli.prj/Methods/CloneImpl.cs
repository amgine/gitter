﻿#region Copyright Notice
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
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.CLI;

	using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

	sealed class CloneImpl : IGitAction<CloneRepositoryParameters>
	{
		private readonly ICommandExecutor _commandExecutor;
		private readonly Func<CloneRepositoryParameters, bool, Command> _commandFactory;

		public CloneImpl(ICommandExecutor commandExecutor, Func<CloneRepositoryParameters, bool, Command> commandFactory)
		{
			_commandExecutor = commandExecutor;
			_commandFactory  = commandFactory;
		}

		/// <summary>Clone existing repository.</summary>
		/// <param name="parameters"><see cref="CloneRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void Invoke(CloneRepositoryParameters parameters)
		{
			/*
			 * git clone [--template=<template_directory>] [-l] [-s] [--no-hardlinks]
			 * [-q] [-n] [--bare] [--mirror] [-o <name>] [-b <name>] [-u <upload-pack>]
			 * [--reference <repository>] [--depth <depth>] [--recursive] [--] <repository> [<directory>]
			*/
			Verify.Argument.IsNotNull(parameters, nameof(parameters));

			try
			{
				if(!Directory.Exists(parameters.Path))
				{
					Directory.CreateDirectory(parameters.Path);
				}
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
				throw new GitException(exc.Message, exc);
			}
			var command = _commandFactory(parameters, false);
			var output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Clone existing repository.</summary>
		/// <param name="parameters"><see cref="CloneRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public async Task InvokeAsync(CloneRepositoryParameters parameters,
			IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			Verify.Argument.IsNotNull(parameters, nameof(parameters));

			var command = _commandFactory(parameters, true);
			if(!Directory.Exists(parameters.Path))
			{
				Directory.CreateDirectory(parameters.Path);
			}
			progress?.Report(new OperationProgress(Resources.StrsConnectingToRemoteHost.AddEllipsis()));

			var errorMessages  = default(List<string>);
			var stdOutReceiver = new NullReader();
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
						if(!string.IsNullOrWhiteSpace(operationProgress.ActionName))
						{
							if(errorMessages == null)
							{
								errorMessages = new List<string>();
							}
							errorMessages.Add(operationProgress.ActionName);
						}
					}
					else
					{
						errorMessages?.Clear();
					}
				}
			};

			var processExitCode = await _commandExecutor
				.ExecuteCommandAsync(command, stdOutReceiver, stdErrReceiver, CommandExecutionFlags.None, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			if(processExitCode != 0)
			{
				var errorMessage = errorMessages != null && errorMessages.Count != 0
					? string.Join(Environment.NewLine, errorMessages)
					: string.Format(CultureInfo.InvariantCulture, "git process exited with code {0}", processExitCode);
				throw new GitException(errorMessage);
			}
		}
	}
}
