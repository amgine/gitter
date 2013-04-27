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
		public static CommandArgument Annotate()
		{
			return new CommandArgument("-a");
		}

		public static CommandArgument SignByEmail()
		{
			return new CommandArgument("-s");
		}

		public static CommandArgument Message(string message)
		{
			return new CommandArgument("-m", "\"" + message + "\"", ' ');
		}

		public static CommandArgument MessageFromFile(string filename)
		{
			return new CommandArgument("-F", filename, ' ');
		}

		public static CommandArgument SignByKey(string keyid)
		{
			return new CommandArgument("-u", keyid, ' ');
		}

		public static CommandArgument Verify()
		{
			return new CommandArgument("-v");
		}

		public static CommandArgument List()
		{
			return new CommandArgument("-l");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("-f");
		}

		public static CommandArgument List(string pattern)
		{
			return new CommandArgument("-l", pattern, ' ');
		}

		public static CommandArgument ReplaceExisting()
		{
			return new CommandArgument("-f");
		}

		public static CommandArgument Delete()
		{
			return new CommandArgument("-d");
		}

		public TagCommand()
			: base("tag")
		{
		}

		public TagCommand(params CommandArgument[] args)
			: base("tag", args)
		{
		}

		public TagCommand(IList<CommandArgument> args)
			: base("tag", args)
		{
		}
	}
}
