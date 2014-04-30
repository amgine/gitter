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
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

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
			var args = new List<ICommandArgument>();
			args.Add(LogCommand.WalkReflogs());
			if(parameters.MaxCount != 0)
			{
				args.Add(LogCommand.MaxCount(parameters.MaxCount));
			}
			args.Add(LogCommand.NullTerminate());
			args.Add(LogCommand.FormatRaw());
			if(parameters.Reference != null)
			{
				args.Add(new CommandParameter(parameters.Reference));
			}
			return new LogCommand(args);
		}

		public IList<ReflogRecordData> Invoke(QueryReflogParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = _commandFactory(parameters);
			var output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();

			var cache = new Dictionary<Hash, RevisionData>(Hash.EqualityComparer);
			var list  = ParseResult1(output, cache);

			// get real commit parents
			command = GetCommand2(parameters);
			output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);

			return list;
		}

		private static IList<ReflogRecordData> ParseResult1(GitOutput output, Dictionary<Hash, RevisionData> cache)
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
				var sha1    = parser.ReadHash(skip: 1);
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

			var task1 = _commandExecutor.ExecuteCommandAsync(command1, CommandExecutionFlags.None, cancellationToken);
			var task2 = _commandExecutor.ExecuteCommandAsync(command2, CommandExecutionFlags.None, cancellationToken);

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

					var cache  = new Dictionary<Hash, RevisionData>(Hash.EqualityComparer);
					var list   = ParseResult1(output1, cache);
					var parser = new GitParser(output2.Output);
					parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);
					return list;
				},
				cancellationToken,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}
	}
}
