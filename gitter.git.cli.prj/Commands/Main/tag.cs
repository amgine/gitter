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
	using System.Collections.Generic;

	/// <summary>Create, list, delete or verify a tag object signed with GPG.</summary>
	public sealed class TagCommand : Command
	{
		public static ICommandArgument Annotate()
		{
			return new CommandFlag("-a");
		}

		public static ICommandArgument SignByEmail()
		{
			return new CommandFlag("-s");
		}

		public static ICommandArgument Message(string message)
		{
			return new CommandParameterValue("-m", "\"" + message + "\"", ' ');
		}

		public static ICommandArgument MessageFromFile(string filename)
		{
			return new CommandParameterValue("-F", filename, ' ');
		}

		public static ICommandArgument SignByKey(string keyid)
		{
			return new CommandParameterValue("-u", keyid, ' ');
		}

		public static ICommandArgument Verify()
		{
			return new CommandFlag("-v");
		}

		public static ICommandArgument List()
		{
			return new CommandFlag("-l");
		}

		public static ICommandArgument Force()
		{
			return new CommandFlag("-f");
		}

		public static ICommandArgument List(string pattern)
		{
			return new CommandParameterValue("-l", pattern, ' ');
		}

		public static ICommandArgument ReplaceExisting()
		{
			return new CommandFlag("-f");
		}

		public static ICommandArgument Delete()
		{
			return new CommandFlag("-d");
		}

		public TagCommand()
			: base("tag")
		{
		}

		public TagCommand(params ICommandArgument[] args)
			: base("tag", args)
		{
		}

		public TagCommand(IList<ICommandArgument> args)
			: base("tag", args)
		{
		}
	}
}
