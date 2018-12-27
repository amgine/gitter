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
	using System.Text;

	/// <summary>Input data for git.exe.</summary>
	internal sealed class GitInput
	{
		#region .ctor

		public GitInput(IList<ICommandArgument> options)
			: this(string.Empty, null, GitProcess.DefaultEncoding, options)
		{
		}

		public GitInput(Command command)
			: this(string.Empty, command, GitProcess.DefaultEncoding, null)
		{
		}

		public GitInput(Command command, Encoding encoding)
			: this(string.Empty, command, encoding, null)
		{
		}

		public GitInput(string workingDirectory, Command command, Encoding encoding = null,
			IList<ICommandArgument> options = null,
			IDictionary<string, string> environment = null)
		{
			WorkingDirectory = workingDirectory ?? string.Empty;
			Command          = command;
			Options          = options;
			Encoding         = encoding ?? GitProcess.DefaultEncoding;
			Environment      = environment;
		}

		#endregion

		#region Properties

		public string WorkingDirectory { get; }

		public Encoding Encoding { get; }

		public IList<ICommandArgument> Options { get; }

		public Command Command { get; }

		public IDictionary<string, string> Environment { get; }

		#endregion

		#region Methods

		public string GetArguments()
		{
			const char OptionSeparator = ' ';

			var sb = new StringBuilder();
			if(Options != null && Options.Count != 0)
			{
				foreach(var opt in Options)
				{
					if(opt == null)
					{
						continue;
					}
					if(sb.Length != 0)
					{
						sb.Append(OptionSeparator);
					}
					opt.ToString(sb);
				}
			}
			if(Command != null)
			{
				if(sb.Length != 0)
				{
					sb.Append(OptionSeparator);
				}
				Command.ToString(sb);
			}
			return sb.ToString();
		}

		#endregion
	}
}
