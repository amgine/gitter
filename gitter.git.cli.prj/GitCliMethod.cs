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
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;
	using gitter.Framework.CLI;

	using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

	static class GitCliMethod
	{
		sealed class GitActionImpl0<TParameters> : IGitAction<TParameters>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Encoding _encoding;
			private readonly Func<TParameters, Command> _commandFactory;

			#endregion

			#region .ctor

			public GitActionImpl0(
				ICommandExecutor commandExecutor,
				Encoding encoding,
				Func<TParameters, Command> commandFactory)
			{
				_commandExecutor = commandExecutor;
				_encoding = encoding;
				_commandFactory = commandFactory;
			}

			#endregion

			#region Methods

			public void Invoke(TParameters parameters)
			{
				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _encoding);
				output.ThrowOnBadReturnCode();
			}

			public Task InvokeAsync(TParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _encoding, cancellationToken)
					.ContinueWith(
					t => TaskUtility.UnwrapResult(t).ThrowOnBadReturnCode(),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		sealed class GitActionImpl1<TParameter> : IGitAction<TParameter>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Encoding _encoding;
			private readonly Func<TParameter, Command> _commandFactory;
			private readonly Action<GitOutput> _resultHandler;

			#endregion

			#region .ctor

			public GitActionImpl1(
				ICommandExecutor commandExecutor,
				Encoding encoding,
				Func<TParameter, Command> commandFactory,
				Action<GitOutput> resultHandler)
			{
				_commandExecutor = commandExecutor;
				_encoding = encoding;
				_commandFactory = commandFactory;
				_resultHandler = resultHandler;
			}

			#endregion

			#region Methods

			public void Invoke(TParameter parameters)
			{
				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _encoding);
				_resultHandler(output);
			}

			public Task InvokeAsync(TParameter parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _encoding, cancellationToken)
					.ContinueWith(
					t => _resultHandler(TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		sealed class GitActionImpl2<TParameter> : IGitAction<TParameter>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Encoding _encoding;
			private readonly Func<TParameter, Command> _commandFactory;
			private readonly Action<TParameter, GitOutput> _resultHandler;

			#endregion

			#region .ctor

			public GitActionImpl2(
				ICommandExecutor commandExecutor,
				Encoding encoding,
				Func<TParameter, Command> commandFactory,
				Action<TParameter, GitOutput> resultHandler)
			{
				_commandExecutor = commandExecutor;
				_encoding = encoding;
				_commandFactory = commandFactory;
				_resultHandler = resultHandler;
			}

			#endregion

			#region Methods

			public void Invoke(TParameter parameters)
			{
				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _encoding);
				_resultHandler(parameters, output);
			}

			public Task InvokeAsync(TParameter parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _encoding, cancellationToken)
					.ContinueWith(
					t => _resultHandler(parameters, TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		sealed class GitFunctionImpl1<TParameter, TOutput> : IGitFunction<TParameter, TOutput>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Encoding _encoding;
			private readonly Func<TParameter, Command> _commandFactory;
			private readonly Func<GitOutput, TOutput> _resultParser;

			#endregion

			#region .ctor

			public GitFunctionImpl1(
				ICommandExecutor commandExecutor,
				Encoding encoding,
				Func<TParameter, Command> commandFactory,
				Func<GitOutput, TOutput> resultParser)
			{
				_commandExecutor = commandExecutor;
				_encoding = encoding;
				_commandFactory = commandFactory;
				_resultParser = resultParser;
			}

			#endregion

			#region Methods

			public TOutput Invoke(TParameter parameters)
			{
				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _encoding);
				return _resultParser(output);
			}

			public Task<TOutput> InvokeAsync(TParameter parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _encoding, cancellationToken)
					.ContinueWith(
					t => _resultParser(TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		sealed class GitFunctionImpl2<TParameter, TOutput> : IGitFunction<TParameter, TOutput>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Encoding _encoding;
			private readonly Func<TParameter, Command> _commandFactory;
			private readonly Func<TParameter, GitOutput, TOutput> _resultParser;

			#endregion

			#region .ctor

			public GitFunctionImpl2(
				ICommandExecutor commandExecutor,
				Encoding encoding,
				Func<TParameter, Command> commandFactory,
				Func<TParameter, GitOutput, TOutput> resultParser)
			{
				_commandExecutor = commandExecutor;
				_encoding = encoding;
				_commandFactory = commandFactory;
				_resultParser = resultParser;
			}

			#endregion

			#region Methods

			public TOutput Invoke(TParameter parameters)
			{
				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command, _encoding);
				return _resultParser(parameters, output);
			}

			public Task<TOutput> InvokeAsync(TParameter parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, _encoding, cancellationToken)
					.ContinueWith(
					t => _resultParser(parameters, TaskUtility.UnwrapResult(t)),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		sealed class GitFunctionImpl3<TParameter> : IGitFunction<TParameter, byte[]>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Encoding _encoding;
			private readonly Func<TParameter, Command> _commandFactory;

			#endregion

			#region .ctor

			public GitFunctionImpl3(
				ICommandExecutor commandExecutor,
				Encoding encoding,
				Func<TParameter, Command> commandFactory)
			{
				_commandExecutor = commandExecutor;
				_encoding = encoding;
				_commandFactory = commandFactory;
			}

			#endregion

			#region Methods

			public byte[] Invoke(TParameter parameters)
			{
				var command = _commandFactory(parameters);
				var stdInReceiver = new AsyncBytesReader();
				var stdErrReceiver = new AsyncTextReader();
				var exitCode = _commandExecutor.ExecuteCommand(command, stdInReceiver, stdErrReceiver);
				if(exitCode != 0)
				{
					throw new GitException(stdErrReceiver.GetText());
				}
				return stdInReceiver.GetBytes();
			}

			public Task<byte[]> InvokeAsync(TParameter parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				var command = _commandFactory(parameters);
				var stdInReceiver = new AsyncBytesReader();
				var stdErrReceiver = new AsyncTextReader();
				return _commandExecutor
					.ExecuteCommandAsync(command, stdInReceiver, stdErrReceiver, cancellationToken)
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

		sealed class FetchOrPullImpl<TParameter> : IGitAction<TParameter>
			where TParameter : FetchParameters
		{
			private readonly ICommandExecutor _commandExecutor;
			private readonly Func<TParameter, bool, Command> _commandFactory;

			public FetchOrPullImpl(ICommandExecutor commandExecutor, Func<TParameter, bool, Command> commandFactory)
			{
				_commandExecutor = commandExecutor;
				_commandFactory = commandFactory;
			}

			public void Invoke(TParameter parameters)
			{
				var command = _commandFactory(parameters, false);
				var output = _commandExecutor.ExecuteCommand(command, null);
				output.ThrowOnBadReturnCode();
			}

			public Task InvokeAsync(TParameter parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
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
					.ExecuteCommandAsync(command, stdOutReceiver, stdErrReceiver, cancellationToken)
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
				var output = _commandExecutor.ExecuteCommand(command);
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
					.ExecuteCommandAsync(command, stdOutReceiver, stdErrReceiver, cancellationToken)
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

		sealed class QuerySymbolicReferenceImpl : IGitFunction<QuerySymbolicReferenceParameters, SymbolicReferenceData>
		{
			private const string refPrefix = "ref: ";

			private readonly IGitRepository _repository;

			public QuerySymbolicReferenceImpl(IGitRepository repository)
			{
				_repository = repository;
			}

			private static SymbolicReferenceData Parse(string value)
			{
				if(value != null && value.Length >= 17 && value.StartsWith(refPrefix + GitConstants.LocalBranchPrefix))
				{
					return new SymbolicReferenceData(value.Substring(16), ReferenceType.LocalBranch);
				}
				else
				{
					if(GitUtils.IsValidSHA1(value))
					{
						return new SymbolicReferenceData(value, ReferenceType.Revision);
					}
				}
				return new SymbolicReferenceData(null, ReferenceType.None);
			}

			public SymbolicReferenceData Invoke(QuerySymbolicReferenceParameters parameters)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var fileName = _repository.GetGitFileName(parameters.Name);
				if(File.Exists(fileName))
				{
					string pointer;
					using(var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						if(fs.Length == 0)
						{
							return new SymbolicReferenceData(null, ReferenceType.None);
						}
						else
						{
							using(var sr = new StreamReader(fs))
							{
								pointer = sr.ReadLine();
								sr.Close();
							}
						}
					}
					return Parse(pointer);
				}
				return new SymbolicReferenceData(null, ReferenceType.None);
			}

			public Task<SymbolicReferenceData> InvokeAsync(QuerySymbolicReferenceParameters parameters,
				IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				return TaskUtility.TaskFromResult(Invoke(parameters));
			}
		}

		sealed class QueryReflogImpl : IGitFunction<QueryReflogParameters, IList<ReflogRecordData>>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Func<QueryReflogParameters, Command> _commandFactory;

			#endregion

			public QueryReflogImpl(ICommandExecutor commandExecutor, Func<QueryReflogParameters, Command> commandFcatory)
			{
				_commandExecutor = commandExecutor;
				_commandFactory = commandFcatory;
			}

			private static Command GetCommand2(QueryReflogParameters parameters)
			{
				var args = new List<CommandArgument>();
				args.Add(LogCommand.WalkReflogs());
				if(parameters.MaxCount != 0)
				{
					args.Add(LogCommand.MaxCount(parameters.MaxCount));
				}
				args.Add(LogCommand.NullTerminate());
				args.Add(LogCommand.FormatRaw());
				if(parameters.Reference != null)
				{
					args.Add(new CommandArgument(parameters.Reference));
				}
				return new LogCommand(args);
			}

			public IList<ReflogRecordData> Invoke(QueryReflogParameters parameters)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command);
				output.ThrowOnBadReturnCode();

				var cache = new Dictionary<string, RevisionData>();
				var list = ParseResult1(output, cache);

				// get real commit parents
				command = GetCommand2(parameters);
				output = _commandExecutor.ExecuteCommand(command);
				output.ThrowOnBadReturnCode();
				var parser = new GitParser(output.Output);
				parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);

				return list;
			}

			private static IList<ReflogRecordData> ParseResult1(GitOutput output, Dictionary<string, RevisionData> cache)
			{
				if(output.Output.Length < 40)
				{
					return new ReflogRecordData[0];
				}
				var parser = new GitParser(output.Output);
				int index = 0;
				var list = new List<ReflogRecordData>();
				while(!parser.IsAtEndOfString)
				{
					var selector = parser.ReadLine();
					if(selector.Length == 0)
					{
						break;
					}
					var message = parser.ReadLine();
					var sha1 = parser.ReadString(40, 1);
					RevisionData rev;
					if(!cache.TryGetValue(sha1, out rev))
					{
						rev = new RevisionData(sha1);
						cache.Add(sha1, rev);
					}
					parser.ParseRevisionData(rev, cache);
					list.Add(new ReflogRecordData(index++, message, rev));
				}
				return list;
			}

			public Task<IList<ReflogRecordData>> InvokeAsync(QueryReflogParameters parameters,
				IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command1 = _commandFactory(parameters);
				var command2 = GetCommand2(parameters);

				var tcs = new TaskCompletionSource<object>();
				if(cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() => tcs.TrySetCanceled());
				}

				int completedTasks = 0;

				var task1 = _commandExecutor.ExecuteCommandAsync(command1, cancellationToken);
				var task2 = _commandExecutor.ExecuteCommandAsync(command2, cancellationToken);

				task1.ContinueWith(
					t => tcs.TrySetCanceled(),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnCanceled,
					TaskScheduler.Default);
				task2.ContinueWith(
					t => tcs.TrySetCanceled(),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnCanceled,
					TaskScheduler.Default);
				task1.ContinueWith(
					t => tcs.TrySetException(t.Exception),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
					TaskScheduler.Default);
				task2.ContinueWith(
					t => tcs.TrySetException(t.Exception),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
					TaskScheduler.Default);
				task1.ContinueWith(
					t =>
					{
						if(Interlocked.Increment(ref completedTasks) == 2)
						{
							tcs.TrySetResult(null);
						}
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
					TaskScheduler.Default);
				task2.ContinueWith(
					t =>
					{
						if(Interlocked.Increment(ref completedTasks) == 2)
						{
							tcs.TrySetResult(null);
						}
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
					TaskScheduler.Default);

				return tcs.Task.
					ContinueWith(
					t =>
					{
						TaskUtility.PropagateFaultedStates(t);

						var output1 = TaskUtility.UnwrapResult(task1);
						output1.ThrowOnBadReturnCode();
						var output2 = TaskUtility.UnwrapResult(task2);
						output2.ThrowOnBadReturnCode();

						var cache = new Dictionary<string, RevisionData>();
						var list = ParseResult1(output1, cache);
						var parser = new GitParser(output2.Output);
						parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);
						return list;
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}
		}

		sealed class QueryStashImpl : IGitFunction<QueryStashParameters, IList<StashedStateData>>
		{
			#region Data

			private readonly ICommandExecutor _commandExecutor;
			private readonly Func<QueryStashParameters, Command> _commandFactory;

			#endregion

			public QueryStashImpl(ICommandExecutor commandExecutor, Func<QueryStashParameters, Command> commandFcatory)
			{
				_commandExecutor = commandExecutor;
				_commandFactory = commandFcatory;
			}

			private static Command GetCommand2(QueryStashParameters parameters)
			{
				return new LogCommand(
					LogCommand.WalkReflogs(),
					LogCommand.NullTerminate(),
					LogCommand.FormatRaw(),
					new CommandArgument(GitConstants.StashFullName));
			}

			public IList<StashedStateData> Invoke(QueryStashParameters parameters)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command);
				output.ThrowOnBadReturnCode();

				var cache = new Dictionary<string, RevisionData>();
				var list = ParseResult1(output, cache);

				// get real commit parents
				command = GetCommand2(parameters);
				output = _commandExecutor.ExecuteCommand(command);
				output.ThrowOnBadReturnCode();
				var parser = new GitParser(output.Output);
				parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);

				return list;
			}

			private static IList<StashedStateData> ParseResult1(GitOutput output, Dictionary<string, RevisionData> cache)
			{
				int index = 0;
				var parser = new GitParser(output.Output);
				var res = new List<StashedStateData>();
				while(!parser.IsAtEndOfString)
				{
					var sha1 = parser.ReadString(40, 1);
					var rev = new RevisionData(sha1);
					parser.ParseRevisionData(rev, cache);
					var state = new StashedStateData(index, rev);
					res.Add(state);
					++index;
				}
				return res;
			}

			public Task<IList<StashedStateData>> InvokeAsync(QueryStashParameters parameters,
				IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				var command1 = _commandFactory(parameters);
				var command2 = GetCommand2(parameters);

				var tcs = new TaskCompletionSource<object>();
				if(cancellationToken.CanBeCanceled)
				{
					cancellationToken.Register(() => tcs.TrySetCanceled());
				}

				int completedTasks = 0;

				var task1 = _commandExecutor.ExecuteCommandAsync(command1, cancellationToken);
				var task2 = _commandExecutor.ExecuteCommandAsync(command2, cancellationToken);

				task1.ContinueWith(
					t => tcs.TrySetCanceled(),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnCanceled,
					TaskScheduler.Default);
				task2.ContinueWith(
					t => tcs.TrySetCanceled(),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnCanceled,
					TaskScheduler.Default);
				task1.ContinueWith(
					t => tcs.TrySetException(t.Exception),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
					TaskScheduler.Default);
				task2.ContinueWith(
					t => tcs.TrySetException(t.Exception),
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
					TaskScheduler.Default);
				task1.ContinueWith(
					t =>
					{
						if(Interlocked.Increment(ref completedTasks) == 2)
						{
							tcs.TrySetResult(null);
						}
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
					TaskScheduler.Default);
				task2.ContinueWith(
					t =>
					{
						if(Interlocked.Increment(ref completedTasks) == 2)
						{
							tcs.TrySetResult(null);
						}
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
					TaskScheduler.Default);

				return tcs.Task.
					ContinueWith(
					t =>
					{
						TaskUtility.PropagateFaultedStates(t);

						var output1 = TaskUtility.UnwrapResult(task1);
						output1.ThrowOnBadReturnCode();
						var output2 = TaskUtility.UnwrapResult(task2);
						output2.ThrowOnBadReturnCode();

						var cache = new Dictionary<string, RevisionData>();
						var list = ParseResult1(output1, cache);
						var parser = new GitParser(output2.Output);
						parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);
						return list;
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}
		}

		sealed class QueryTagMessageImpl : IGitFunction<string, string>
		{
			private readonly ICommandExecutor _commandExecutor;
			private readonly Func<string, Command> _commandFactory;

			public QueryTagMessageImpl(ICommandExecutor commandExecutor, Func<string, Command> commandFactory)
			{
				_commandExecutor = commandExecutor;
				_commandFactory = commandFactory;
			}

			private string ParseTagMessage(Command command, GitOutput output)
			{
				Assert.IsNotNull(output);

				output.ThrowOnBadReturnCode();
				var parser = new GitParser(output.Output);
				while(!parser.IsAtEndOfLine)
				{
					parser.SkipLine();
				}
				parser.SkipLine();
				if(parser.RemainingSymbols > 1)
				{
					var message = parser.ReadStringUpTo(parser.Length - 1);
					const char c = '�';
					if(message.ContainsAnyOf(c))
					{
						output = _commandExecutor.ExecuteCommand(command, Encoding.Default);
						output.ThrowOnBadReturnCode();
						parser = new GitParser(output.Output);
						while(!parser.IsAtEndOfLine)
						{
							parser.SkipLine();
						}
						parser.SkipLine();
						if(parser.RemainingSymbols > 1)
						{
							message = parser.ReadStringUpTo(parser.Length - 1);
						}
						else
						{
							message = string.Empty;
						}
					}
					return message;
				}
				else
				{
					return string.Empty;
				}
			}

			#region IGitFunction<string,string> Members

			public string Invoke(string parameters)
			{
				var command = _commandFactory(parameters);
				var output = _commandExecutor.ExecuteCommand(command);
				return ParseTagMessage(command, output);
			}

			public Task<string> InvokeAsync(string parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				var command = _commandFactory(parameters);
				return _commandExecutor
					.ExecuteCommandAsync(command, cancellationToken)
					.ContinueWith(
					t =>
					{
						var output = TaskUtility.UnwrapResult(t);
						return ParseTagMessage(command, output);
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
			}

			#endregion
		}

		sealed class FormatMergeMessageImpl : IGitFunction<FormatMergeMessageParameters, string>
		{
			private const string changeLogFormat = "  * %s\r";

			private readonly ICommandExecutor _commandExecutor;

			public FormatMergeMessageImpl(ICommandExecutor commandExecutor)
			{
				_commandExecutor = commandExecutor;
			}

			private string GetMergedCommits(string branch, string head, string lineFormat)
			{
				var cmd = new LogCommand(
					LogCommand.TFormat(lineFormat),
					new CommandArgument(head + ".." + branch));
				var output = _commandExecutor.ExecuteCommand(cmd);
				output.ThrowOnBadReturnCode();
				return output.Output;
			}

			public string Invoke(FormatMergeMessageParameters parameters)
			{
				Verify.Argument.IsNotNull(parameters, "parameters");

				if(parameters.Revisions.Count == 1)
				{
					var rev = parameters.Revisions[0];
					var commits = GetMergedCommits(rev, parameters.HeadReference, changeLogFormat);
					var msg = string.Format("Merge branch '{0}' into {1}\r\n\r\n", rev, parameters.HeadReference) + "Changes:\r\n" + commits;
					return msg;
				}
				else
				{
					var sb = new StringBuilder();
					sb.Append("Merge branches ");
					for(int i = 0; i < parameters.Revisions.Count; ++i)
					{
						sb.Append('\'');
						sb.Append(parameters.Revisions[i]);
						sb.Append('\'');
						if(i != parameters.Revisions.Count - 1)
						{
							sb.Append(", ");
						}
					}
					sb.Append(" into ");
					sb.Append(parameters.HeadReference);
					sb.Append("\r\n");
					for(int i = 0; i < parameters.Revisions.Count; ++i)
					{
						sb.Append("\r\nChanges from ");
						sb.Append(parameters.Revisions[i]);
						sb.Append(":\r\n");
						sb.Append(GetMergedCommits(parameters.Revisions[i], parameters.HeadReference, changeLogFormat));
					}
					return sb.ToString();
				}
			}

			public Task<string> InvokeAsync(FormatMergeMessageParameters parameters,
				IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				return Task.Factory.StartNew(
					() => Invoke(parameters),
					cancellationToken,
					TaskCreationOptions.None,
					TaskScheduler.Default);
			}
		}

		public static void Create<TParameters>(
			out IGitAction<TParameters> action,
			ICommandExecutor commandExecutor,
			Func<TParameters, Command> commandFactory,
			Encoding encoding = null)
		{
			action = new GitActionImpl0<TParameters>(
				commandExecutor, encoding, commandFactory);
		}

		public static void Create<TParameters>(
			out IGitAction<TParameters> action,
			ICommandExecutor commandExecutor,
			Func<TParameters, Command> commandFactory,
			Action<GitOutput> resultHandler,
			Encoding encoding = null)
		{
			action = new GitActionImpl1<TParameters>(
				commandExecutor, encoding, commandFactory, resultHandler);
		}

		public static void Create<TParameters>(
			out IGitAction<TParameters> action,
			ICommandExecutor commandExecutor,
			Func<TParameters, Command> commandFactory,
			Action<TParameters, GitOutput> resultHandler,
			Encoding encoding = null)
		{
			action = new GitActionImpl2<TParameters>(
				commandExecutor, encoding, commandFactory, resultHandler);
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
			Encoding encoding = null)
		{
			function = new GitFunctionImpl1<TParameters, TOutput>(
				commandExecutor, encoding, commandFactory, resultParser);
		}

		public static void Create<TParameters, TOutput>(
			out IGitFunction<TParameters, TOutput> function,
			ICommandExecutor commandExecutor,
			Func<TParameters, Command> commandFactory,
			Func<TParameters, GitOutput, TOutput> resultParser,
			Encoding encoding = null)
		{
			function = new GitFunctionImpl2<TParameters, TOutput>(
				commandExecutor, encoding, commandFactory, resultParser);
		}

		public static void Create<TParameters>(
			out IGitFunction<TParameters, byte[]> function,
			ICommandExecutor commandExecutor,
			Func<TParameters, Command> commandFactory,
			Encoding encoding = null)
		{
			function = new GitFunctionImpl3<TParameters>(
				commandExecutor, encoding, commandFactory);
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
			out IGitFunction<string, string> function,
			ICommandExecutor commandExecutor,
			Func<string, Command> commandFactory)
		{
			function = new QueryTagMessageImpl(commandExecutor, commandFactory);
		}

		public static void Create(
			out IGitFunction<FormatMergeMessageParameters, string> function,
			ICommandExecutor commandExecutor)
		{
			function = new FormatMergeMessageImpl(commandExecutor);
		}
	}
}
