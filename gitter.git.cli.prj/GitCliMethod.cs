#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.CLI;

static class GitCliMethod
{
	sealed class GitActionImpl0<TParameters> : IGitAction<TParameters>
		where TParameters : class
	{
		#region Data

		private readonly ICommandExecutor           _commandExecutor;
		private readonly Func<TParameters, Command> _commandFactory;
		private readonly CommandExecutionFlags      _flags;

		#endregion

		#region .ctor

		public GitActionImpl0(
			ICommandExecutor           commandExecutor,
			Func<TParameters, Command> commandFactory,
			CommandExecutionFlags      flags)
		{
			_commandExecutor = commandExecutor;
			_commandFactory  = commandFactory;
			_flags           = flags;
		}

		#endregion

		#region Methods

		public void Invoke(TParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var output = _commandExecutor.ExecuteCommand(command, _flags);
			output.ThrowOnBadReturnCode();
		}

		public async Task InvokeAsync(TParameters parameters, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var output  = await _commandExecutor
				.ExecuteCommandAsync(command, _flags, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			output.ThrowOnBadReturnCode();
		}

		#endregion
	}

	sealed class GitActionImpl1<TParameters> : IGitAction<TParameters>
		where TParameters : class
	{
		#region Data

		private readonly ICommandExecutor           _commandExecutor;
		private readonly Func<TParameters, Command> _commandFactory;
		private readonly Action<GitOutput>          _resultHandler;
		private readonly CommandExecutionFlags      _flags;

		#endregion

		#region .ctor

		public GitActionImpl1(
			ICommandExecutor           commandExecutor,
			Func<TParameters, Command> commandFactory,
			Action<GitOutput>          resultHandler,
			CommandExecutionFlags      flags)
		{
			_commandExecutor = commandExecutor;
			_commandFactory  = commandFactory;
			_resultHandler   = resultHandler;
			_flags           = flags;
		}

		#endregion

		#region Methods

		public void Invoke(TParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var output = _commandExecutor.ExecuteCommand(command, _flags);
			_resultHandler(output);
		}

		public async Task InvokeAsync(TParameters parameters, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var result = await _commandExecutor
				.ExecuteCommandAsync(command, _flags, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			_resultHandler(result);
		}

		#endregion
	}

	sealed class GitActionImpl2<TParameters> : IGitAction<TParameters>
		where TParameters : class
	{
		#region Data

		private readonly ICommandExecutor               _commandExecutor;
		private readonly Func<TParameters, Command>     _commandFactory;
		private readonly Action<TParameters, GitOutput> _resultHandler;
		private readonly CommandExecutionFlags          _flags;

		#endregion

		#region .ctor

		public GitActionImpl2(
			ICommandExecutor               commandExecutor,
			Func<TParameters, Command>     commandFactory,
			Action<TParameters, GitOutput> resultHandler,
			CommandExecutionFlags          flags)
		{
			_commandExecutor = commandExecutor;
			_commandFactory  = commandFactory;
			_resultHandler   = resultHandler;
			_flags           = flags;
		}

		#endregion

		#region Methods

		public void Invoke(TParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var output = _commandExecutor.ExecuteCommand(command, _flags);
			_resultHandler(parameters, output);
		}

		public async Task InvokeAsync(TParameters parameters, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var result = await _commandExecutor
				.ExecuteCommandAsync(command, _flags, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			_resultHandler(parameters, result);
		}

		#endregion
	}

	sealed class GitFunctionImpl0<TParameters, TOutput> : IGitFunction<TParameters, TOutput>
		where TParameters : class
	{
		#region Data

		private readonly ICommandExecutor           _commandExecutor;
		private readonly Func<TParameters, Command> _commandFactory;
		private readonly Func<GitOutput, TOutput>   _resultParser;
		private readonly CommandExecutionFlags      _flags;

		#endregion

		#region .ctor

		public GitFunctionImpl0(
			ICommandExecutor           commandExecutor,
			Func<TParameters, Command> commandFactory,
			Func<GitOutput, TOutput>   resultParser,
			CommandExecutionFlags      flags)
		{
			_commandExecutor = commandExecutor;
			_commandFactory  = commandFactory;
			_resultParser    = resultParser;
			_flags           = flags;
		}

		#endregion

		#region Methods

		public TOutput Invoke(TParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var output = _commandExecutor.ExecuteCommand(command, _flags);
			return _resultParser(output);
		}

		public async Task<TOutput> InvokeAsync(TParameters parameters, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var result = await _commandExecutor
				.ExecuteCommandAsync(command, _flags, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			return _resultParser(result);
		}

		#endregion
	}

	sealed class GitFunctionImpl1<TParameters, TOutput> : IGitFunction<TParameters, TOutput>
		where TParameters : class
	{
		#region Data

		private readonly ICommandExecutor                      _commandExecutor;
		private readonly Func<TParameters, Command>            _commandFactory;
		private readonly Func<TParameters, GitOutput, TOutput> _resultParser;
		private readonly CommandExecutionFlags                 _flags;

		#endregion

		#region .ctor

		public GitFunctionImpl1(
			ICommandExecutor                      commandExecutor,
			Func<TParameters, Command>            commandFactory,
			Func<TParameters, GitOutput, TOutput> resultParser,
			CommandExecutionFlags                 flags)
		{
			_commandExecutor = commandExecutor;
			_commandFactory  = commandFactory;
			_resultParser    = resultParser;
			_flags           = flags;
		}

		#endregion

		#region Methods

		public TOutput Invoke(TParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var output  = _commandExecutor.ExecuteCommand(command, _flags);
			return _resultParser(parameters, output);
		}

		public async Task<TOutput> InvokeAsync(TParameters parameters, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters);

			var command = _commandFactory(parameters);
			var result = await _commandExecutor
				.ExecuteCommandAsync(command, _flags, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			return _resultParser(parameters, result);
		}

		#endregion
	}

	sealed class GitFunctionImpl2<TRequest>(
		ICommandExecutor        commandExecutor,
		Func<TRequest, Command> commandFactory,
		CommandExecutionFlags   flags) : IGitFunction<TRequest, byte[]>
		where TRequest : class
	{
		private static byte[] ExtractOutput(int exitCode, AsyncBytesReader stdInReceiver, AsyncTextReader stdErrReceiver)
		{
			if(exitCode != 0)
			{
				throw new GitException(stdErrReceiver.GetText());
			}
			return stdInReceiver.GetBytes();
		}

		public byte[] Invoke(TRequest request)
		{
			Verify.Argument.IsNotNull(request);

			var command        = commandFactory(request);
			var stdInReceiver  = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			var exitCode = commandExecutor.ExecuteCommand(
				command,
				stdInReceiver,
				stdErrReceiver,
				flags);
			return ExtractOutput(exitCode, stdInReceiver, stdErrReceiver);
		}

		public async Task<byte[]> InvokeAsync(TRequest request, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(request);

			var command        = commandFactory(request);
			var stdInReceiver  = new AsyncBytesReader();
			var stdErrReceiver = new AsyncTextReader();
			var exitCode = await commandExecutor
				.ExecuteCommandAsync(
					command,
					stdInReceiver,
					stdErrReceiver,
					flags,
					cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			return ExtractOutput(exitCode, stdInReceiver, stdErrReceiver);
		}
	}

	public static void Create<TRequest>(
		out IGitAction<TRequest> action,
		ICommandExecutor commandExecutor,
		Func<TRequest, Command> commandFactory,
		CommandExecutionFlags flags = CommandExecutionFlags.None)
		where TRequest : class
	{
		action = new GitActionImpl0<TRequest>(
			commandExecutor, commandFactory, flags);
	}

	public static void Create<TRequest>(
		out IGitAction<TRequest> action,
		ICommandExecutor commandExecutor,
		Func<TRequest, Command> commandFactory,
		Action<GitOutput> resultHandler,
		CommandExecutionFlags flags = CommandExecutionFlags.None)
		where TRequest : class
	{
		action = new GitActionImpl1<TRequest>(
			commandExecutor, commandFactory, resultHandler, flags);
	}

	public static void Create<TParameters>(
		out IGitAction<TParameters> action,
		ICommandExecutor commandExecutor,
		Func<TParameters, Command> commandFactory,
		Action<TParameters, GitOutput> resultHandler,
		CommandExecutionFlags flags = CommandExecutionFlags.None)
		where TParameters : class
	{
		action = new GitActionImpl2<TParameters>(
			commandExecutor, commandFactory, resultHandler, flags);
	}

	public static void Create(
		out IGitAction<FetchRequest> action,
		ICommandExecutor commandExecutor,
		Func<FetchRequest, bool, Command> commandFactory)
	{
		action = new FetchOrPullAction<FetchRequest>(
			commandExecutor, commandFactory);
	}

	public static void Create(
		out IGitAction<PullRequest> action,
		ICommandExecutor commandExecutor,
		Func<PullRequest, bool, Command> commandFactory)
	{
		action = new FetchOrPullAction<PullRequest>(
			commandExecutor, commandFactory);
	}

	public static void Create(
		out IGitAction<CloneRepositoryRequest> action,
		ICommandExecutor commandExecutor,
		Func<CloneRepositoryRequest, bool, Command> commandFactory)
	{
		action = new CloneAction(
			commandExecutor, commandFactory);
	}

	public static void Create(
		out IGitAction<InitRepositoryRequest> action,
		GitCLI gitCLI,
		Func<InitRepositoryRequest, Command> commandFactory)
	{
		action = new InitAction(
			gitCLI, commandFactory);
	}

	public static void Create<TParameters, TOutput>(
		out IGitFunction<TParameters, TOutput> function,
		ICommandExecutor commandExecutor,
		Func<TParameters, Command> commandFactory,
		Func<GitOutput, TOutput> resultParser,
		CommandExecutionFlags flags = CommandExecutionFlags.None)
		where TParameters : class
	{
		function = new GitFunctionImpl0<TParameters, TOutput>(
			commandExecutor, commandFactory, resultParser, flags);
	}

	public static void Create<TParameters, TOutput>(
		out IGitFunction<TParameters, TOutput> function,
		ICommandExecutor commandExecutor,
		Func<TParameters, Command> commandFactory,
		Func<TParameters, GitOutput, TOutput> resultParser,
		CommandExecutionFlags flags = CommandExecutionFlags.None)
		where TParameters : class
	{
		function = new GitFunctionImpl1<TParameters, TOutput>(
			commandExecutor, commandFactory, resultParser, flags);
	}

	public static void Create<TParameters>(
		out IGitFunction<TParameters, byte[]> function,
		ICommandExecutor commandExecutor,
		Func<TParameters, Command> commandFactory,
		CommandExecutionFlags flags = CommandExecutionFlags.None)
		where TParameters : class
	{
		function = new GitFunctionImpl2<TParameters>(
			commandExecutor, commandFactory, flags);
	}

	public static void Create(
		out IGitFunction<PushRequest, Many<ReferencePushResult>> function,
		ICommandExecutor commandExecutor,
		Func<PushRequest, bool, Command> commandFactory,
		Func<string, Many<ReferencePushResult>> resultsParser)
	{
		function = new PushFunction(
			commandExecutor, commandFactory, resultsParser);
	}

	public static void Create(
		out IGitFunction<QuerySymbolicReferenceRequest, SymbolicReferenceData> function,
		IGitRepository repository)
	{
		function = new QuerySymbolicReferenceFunction(repository);
	}

	public static void Create(
		out IGitFunction<QueryReflogRequest, IList<ReflogRecordData>> function,
		ICommandExecutor commandExecutor,
		Func<QueryReflogRequest, Command> commandFactory)
	{
		function = new QueryReflogFunction(commandExecutor, commandFactory);
	}

	public static void Create(
		out IGitFunction<QueryStashRequest, IList<StashedStateData>> function,
		ICommandExecutor commandExecutor,
		Func<QueryStashRequest, Command> commandFactory)
	{
		function = new QueryStashFunction(commandExecutor, commandFactory);
	}

	public static void Create(
		out IGitFunction<QueryTagMessageRequest, string> function,
		ICommandExecutor commandExecutor,
		Func<QueryTagMessageRequest, Command> commandFactory)
	{
		function = new QueryTagMessageFunction(commandExecutor, commandFactory);
	}

	public static void Create(
		out IGitFunction<FormatMergeMessageRequest, string> function,
		ICommandExecutor commandExecutor)
	{
		function = new FormatMergeMessageFunction(commandExecutor);
	}

	public static void Create(
		out IGitFunction<QueryRevisionsRequest, IList<RevisionData>> function,
		ICommandExecutor commandExecutor,
		CommandBuilder commandBuilder)
	{
		function = new QueryRevisionsFunction(commandExecutor, commandBuilder);
	}
}
