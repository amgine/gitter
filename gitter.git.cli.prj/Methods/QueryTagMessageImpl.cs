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
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	sealed class QueryTagMessageImpl : IGitFunction<QueryTagMessageParameters, string>
	{
		#region Data

		private readonly ICommandExecutor _commandExecutor;
		private readonly Func<QueryTagMessageParameters, Command> _commandFactory;

		#endregion

		#region .ctor

		public QueryTagMessageImpl(ICommandExecutor commandExecutor, Func<QueryTagMessageParameters, Command> commandFactory)
		{
			_commandExecutor = commandExecutor;
			_commandFactory = commandFactory;
		}

		#endregion

		#region Methods

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
					output = _commandExecutor.ExecuteCommand(command, Encoding.Default, CommandExecutionFlags.None);
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

		public string Invoke(QueryTagMessageParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = _commandFactory(parameters);
			var output = _commandExecutor.ExecuteCommand(command, CommandExecutionFlags.None);
			return ParseTagMessage(command, output);
		}

		public Task<string> InvokeAsync(QueryTagMessageParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var command = _commandFactory(parameters);
			return _commandExecutor
				.ExecuteCommandAsync(command, CommandExecutionFlags.None, cancellationToken)
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
}
