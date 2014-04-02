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
		#region Data

		private readonly string _workingDirectory;
		private readonly Command _command;
		private readonly Encoding _encoding;
		private IDictionary<string, string> _environment;
		private IList<ICommandArgument> _options;

		#endregion

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

		public GitInput(string workingDirectory, Command command)
			: this(workingDirectory, command, GitProcess.DefaultEncoding, null)
		{
		}

		public GitInput(string workingDirectory, Command command, Encoding encoding)
			: this(workingDirectory, command, encoding, null)
		{
		}

		public GitInput(string workingDirectory, IList<ICommandArgument> options)
			: this(workingDirectory, null, GitProcess.DefaultEncoding, options)
		{
		}

		public GitInput(string workingDirectory, Command command, IList<ICommandArgument> options)
			: this(workingDirectory, command, GitProcess.DefaultEncoding, options)
		{
		}

		public GitInput(string workingDirectory, Command command, Encoding encoding, IList<ICommandArgument> options)
		{
			if(encoding == null)
			{
				encoding = GitProcess.DefaultEncoding;
			}

			_workingDirectory = workingDirectory ?? string.Empty;
			_command          = command;
			_options          = options;
			_encoding         = encoding;
		}

		#endregion

		#region Properties

		public string WorkingDirectory
		{
			get { return _workingDirectory; }
		}

		public Encoding Encoding
		{
			get { return _encoding; }
		}

		public IList<ICommandArgument> Options
		{
			get { return _options; }
			set { _options = value; }
		}

		public Command Command
		{
			get { return _command; }
		}

		public IDictionary<string, string> Environment
		{
			get { return _environment; }
			set { _environment = value; }
		}

		#endregion

		#region Methods

		public string GetArguments()
		{
			var sb = new StringBuilder();
			if(_options != null && _options.Count != 0)
			{
				const char OptionSeparator = ' ';
				bool first = true;
				foreach(var opt in _options)
				{
					if(opt == null)
					{
						continue;
					}
					if(!first)
					{
						sb.Append(OptionSeparator);
					}
					else
					{
						first = false;
					}
					opt.ToString(sb);
				}
			}
			if(_command != null)
			{
				_command.ToString(sb);
			}
			return sb.ToString();
		}

		#endregion
	}
}
