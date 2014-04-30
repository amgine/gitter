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

	sealed class QueryStashImpl : IGitFunction<QueryStashParameters, IList<StashedStateData>>
	{
		#region Data

		private readonly ICommandExecutor _commandExecutor;
		private readonly Func<QueryStashParameters, Command> _commandFactory;

		#endregion

		public QueryStashImpl(ICommandExecutor commandExecutor, Func<QueryStashParameters, Command> commandFactory)
		{
			_commandExecutor = commandExecutor;
			_commandFactory = commandFactory;
		}

		private static Command GetCommand2(QueryStashParameters parameters)
		{
			return new LogCommand(
				LogCommand.WalkReflogs(),
				LogCommand.NullTerminate(),
				LogCommand.FormatRaw(),
				new CommandParameter(GitConstants.StashFullName));
		}

		public IList<StashedStateData> Invoke(QueryStashParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = _commandFactory(parameters);
			var output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();

			var cache = new Dictionary<Hash, RevisionData>(Hash.EqualityComparer);
			var list = ParseResult1(output, cache);

			// get real commit parents
			command = GetCommand2(parameters);
			output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			output.ThrowOnBadReturnCode();
			var parser = new GitParser(output.Output);
			parser.ParseCommitParentsFromRaw(list.Select(rrd => rrd.Revision), cache);

			return list;
		}

		private static IList<StashedStateData> ParseResult1(GitOutput output, Dictionary<Hash, RevisionData> cache)
		{
			int index = 0;
			var parser = new GitParser(output.Output);
			var res = new List<StashedStateData>();
			while(!parser.IsAtEndOfString)
			{
				var sha1 = new Hash(parser.String, parser.Position);
				var rev  = new RevisionData(sha1);
				parser.Skip(41);
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

					var cache = new Dictionary<Hash, RevisionData>(Hash.EqualityComparer);
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
}
