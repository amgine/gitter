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

namespace gitter.Git.AccessLayer.CLI
{
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
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _flags);
				output.ThrowOnBadReturnCode();
			}

			public Task InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _flags, cancellationToken)
					.ContinueWith(
					t => TaskUtility.UnwrapResult(t).ThrowOnBadReturnCode(),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
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
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _flags);
				_resultHandler(output);
			}

			public Task InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _flags, cancellationToken)
					.ContinueWith(
					t => _resultHandler(TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
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
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _flags);
				_resultHandler(parameters, output);
			}

			public Task InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _flags, cancellationToken)
					.ContinueWith(
					t => _resultHandler(parameters, TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
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
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _flags);
				return _resultParser(output);
			}

			public Task<TOutput> InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _flags, cancellationToken)
					.ContinueWith(
					t => _resultParser(TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
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
			private readonly CommandExecutionFlags                        _flags;

			#endregion

			#region .ctor

			public GitFunctionImpl1(
				ICommandExecutor                      commandExecutor,
				Func<TParameters, Command>            commandFactory,
				Func<TParameters, GitOutput, TOutput> resultParser,
				CommandExecutionFlags                        flags)
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
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var output  = _commandExecutor.ExecuteCommand(command, _flags);
				return _resultParser(parameters, output);
			}

			public Task<TOutput> InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _flags, cancellationToken)
					.ContinueWith(
					t => _resultParser(parameters, TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		sealed class GitFunctionImpl2<TParameters> : IGitFunction<TParameters, byte[]>
			where TParameters : class
		{
			#region Data

			private readonly ICommandExecutor           _commandExecutor;
			private readonly Func<TParameters, Command> _commandFactory;
			private readonly CommandExecutionFlags      _flags;

			#endregion

			#region .ctor

			public GitFunctionImpl2(
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

			public byte[] Invoke(TParameters parameters)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var stdInReceiver = new AsyncBytesReader();
				var stdErrReceiver = new AsyncTextReader();
				var exitCode = _commandExecutor.ExecuteCommand(
					command,
					stdInReceiver,
					stdErrReceiver,
					CommandExecutionFlags.None);
				if(exitCode != 0)
				{
					throw new GitException(stdErrReceiver.GetText());
				}
				return stdInReceiver.GetBytes();
			}

			public Task<byte[]> InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var stdInReceiver = new AsyncBytesReader();
				var stdErrReceiver = new AsyncTextReader();
				return _commandExecutor
					.ExecuteCommandAsync(
						command,
						stdInReceiver,
						stdErrReceiver,
						CommandExecutionFlags.None,
						cancellationToken)
					.ContinueWith(
					t =>
					{
						var exitCode = TaskUtility.UnwrapResult(t);
						if(exitCode != 0)
						{
							throw new GitException(stdErrReceiver.GetText());
						}
						return stdInReceiver.GetBytes();
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		public static void Create<TParameters>(
			out IGitAction<TParameters> action,
			ICommandExecutor commandExecutor,
			Func<TParameters, Command> commandFactory,
			CommandExecutionFlags flags = CommandExecutionFlags.None)
			where TParameters : class
		{
			action = new GitActionImpl0<TParameters>(
				commandExecutor, commandFactory, flags);
		}

		public static void Create<TParameters>(
			out IGitAction<TParameters> action,
			ICommandExecutor commandExecutor,
			Func<TParameters, Command> commandFactory,
			Action<GitOutput> resultHandler,
			CommandExecutionFlags flags = CommandExecutionFlags.None)
			where TParameters : class
		{
			action = new GitActionImpl1<TParameters>(
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
			out IGitAction<FetchParameters> action,
			ICommandExecutor commandExecutor,
			Func<FetchParameters, bool, Command> commandFactory)
		{
			action = new FetchOrPullImpl<FetchParameters>(
				commandExecutor, commandFactory);
		}

		public static void Create(
			out IGitAction<PullParameters> action,
			ICommandExecutor commandExecutor,
			Func<PullParameters, bool, Command> commandFactory)
		{
			action = new FetchOrPullImpl<PullParameters>(
				commandExecutor, commandFactory);
		}

		public static void Create(
			out IGitAction<CloneRepositoryParameters> action,
			ICommandExecutor commandExecutor,
			Func<CloneRepositoryParameters, bool, Command> commandFactory)
		{
			action = new CloneImpl(
				commandExecutor, commandFactory);
		}

		public static void Create(
			out IGitAction<InitRepositoryParameters> action,
			GitCLI gitCLI,
			Func<InitRepositoryParameters, Command> commandFactory)
		{
			action = new InitImpl(
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
			out IGitFunction<PushParameters, IList<ReferencePushResult>> function,
			ICommandExecutor commandExecutor,
			Func<PushParameters, bool, Command> commandFactory,
			Func<string, IList<ReferencePushResult>> resultsParser)
		{
			function = new PushImpl(
				commandExecutor, commandFactory, resultsParser);
		}

		public static void Create(
			out IGitFunction<QuerySymbolicReferenceParameters, SymbolicReferenceData> function,
			IGitRepository repository)
		{
			function = new QuerySymbolicReferenceImpl(repository);
		}

		public static void Create(
			out IGitFunction<QueryReflogParameters, IList<ReflogRecordData>> function,
			ICommandExecutor commandExecutor,
			Func<QueryReflogParameters, Command> commandFactory)
		{
			function = new QueryReflogImpl(commandExecutor, commandFactory);
		}

		public static void Create(
			out IGitFunction<QueryStashParameters, IList<StashedStateData>> function,
			ICommandExecutor commandExecutor,
			Func<QueryStashParameters, Command> commandFactory)
		{
			function = new QueryStashImpl(commandExecutor, commandFactory);
		}

		public static void Create(
			out IGitFunction<QueryTagMessageParameters, string> function,
			ICommandExecutor commandExecutor,
			Func<QueryTagMessageParameters, Command> commandFactory)
		{
			function = new QueryTagMessageImpl(commandExecutor, commandFactory);
		}

		public static void Create(
			out IGitFunction<FormatMergeMessageParameters, string> function,
			ICommandExecutor commandExecutor)
		{
			function = new FormatMergeMessageImpl(commandExecutor);
		}

		public static void Create(
			out IGitFunction<QueryRevisionsParameters, IList<RevisionData>> function,
			ICommandExecutor commandExecutor,
			CommandBuilder commandBuilder)
		{
			function = new QueryRevisionsImpl(commandExecutor, commandBuilder);
		}
	}
}
