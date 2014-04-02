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
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

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
				new CommandParameter(head + ".." + branch));
			var output = _commandExecutor.ExecuteCommand(cmd, CommandExecutionFlags.None);
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
}
