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
		public static ICommandArgument List()
		{
			return new CommandFlag("--list");
		}

		public static ICommandArgument NullTerminate()
		{
			return new CommandFlag("--null");
		}

		public static ICommandArgument Add()
		{
			return new CommandFlag("--add");
		}

		public static ICommandArgument Unset()
		{
			return new CommandFlag("--unset");
		}

		public static ICommandArgument UnsetAll()
		{
			return new CommandFlag("--unset-all");
		}

		public static ICommandArgument Get(string name)
		{
			return new CommandParameterValue("--get", name, ' ');
		}

		public static ICommandArgument GetAll(string name)
		{
			return new CommandParameterValue("--get-all", name, ' ');
		}

		public static ICommandArgument GetRegexp(string nameregex)
		{
			return new CommandParameterValue("--get-regexp", nameregex, ' ');
		}

		public static ICommandArgument Global()
		{
			return new CommandFlag("--global");
		}

		public static ICommandArgument System()
		{
			return new CommandFlag("--system");
		}

		public static ICommandArgument Int()
		{
			return new CommandFlag("--int");
		}

		public static ICommandArgument Path()
		{
			return new CommandFlag("--path");
		}

		public static ICommandArgument Edit()
		{
			return new CommandFlag("--edit");
		}

		public static ICommandArgument Bool()
		{
			return new CommandFlag("--bool");
		}

		public static ICommandArgument BoolOrInt()
		{
			return new CommandFlag("--bool-or-int");
		}

		public static ICommandArgument File(string fileName)
		{
			return new CommandParameterValue("--file", fileName, ' ');
		}

		public static ICommandArgument RemoveSection(string name)
		{
			return new CommandParameterValue("--remove-section", name, ' ');
		}

		public static ICommandArgument RenameSection(string oldName, string newName)
		{
			return new CommandParameterValue("--rename-section", oldName + " " + newName, ' ');
		}

		public ConfigCommand()
			: base("config")
		{
		}

		public ConfigCommand(params ICommandArgument[] args)
			: base("config", args)
		{
		}

		public ConfigCommand(IList<ICommandArgument> args)
			: base("config", args)
		{
		}
	}
}
