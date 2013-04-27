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

	/// <summary>Get and set repository or global options.</summary>
	public sealed class ConfigCommand : Command
	{
		public static CommandArgument List()
		{
			return new CommandArgument("--list");
		}

		public static CommandArgument NullTerminate()
		{
			return new CommandArgument("--null");
		}

		public static CommandArgument Add()
		{
			return new CommandArgument("--add");
		}

		public static CommandArgument Unset()
		{
			return new CommandArgument("--unset");
		}

		public static CommandArgument UnsetAll()
		{
			return new CommandArgument("--unset-all");
		}

		public static CommandArgument Get(string name)
		{
			return new CommandArgument("--get", name, ' ');
		}

		public static CommandArgument GetAll(string name)
		{
			return new CommandArgument("--get-all", name, ' ');
		}

		public static CommandArgument GetRegexp(string nameregex)
		{
			return new CommandArgument("--get-regexp", nameregex, ' ');
		}

		public static CommandArgument Global()
		{
			return new CommandArgument("--global");
		}

		public static CommandArgument System()
		{
			return new CommandArgument("--system");
		}

		public static CommandArgument Int()
		{
			return new CommandArgument("--int");
		}

		public static CommandArgument Path()
		{
			return new CommandArgument("--path");
		}

		public static CommandArgument Edit()
		{
			return new CommandArgument("--edit");
		}

		public static CommandArgument Bool()
		{
			return new CommandArgument("--bool");
		}

		public static CommandArgument BoolOrInt()
		{
			return new CommandArgument("--bool-or-int");
		}

		public static CommandArgument File(string fileName)
		{
			return new CommandArgument("--file", fileName, ' ');
		}

		public static CommandArgument RemoveSection(string name)
		{
			return new CommandArgument("--remove-section", name, ' ');
		}

		public static CommandArgument RenameSection(string oldName, string newName)
		{
			return new CommandArgument("--rename-section", oldName + " " + newName, ' ');
		}

		public ConfigCommand()
			: base("config")
		{
		}

		public ConfigCommand(params CommandArgument[] args)
			: base("config", args)
		{
		}

		public ConfigCommand(IList<CommandArgument> args)
			: base("config", args)
		{
		}
	}
}
