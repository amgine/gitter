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

	/// <summary>Add or inspect object notes.</summary>
	public sealed class NotesCommand : Command
	{
		public static ICommandArgument List()
		{
			return new CommandParameter("list");
		}

		public static ICommandArgument List(string @object)
		{
			return new CommandParameterValue("list", @object, ' ');
		}

		public static ICommandArgument Add()
		{
			return new CommandParameter("add");
		}

		public static ICommandArgument Copy()
		{
			return new CommandParameter("copy");
		}

		public static ICommandArgument Append()
		{
			return new CommandParameter("append");
		}

		public static ICommandArgument Edit()
		{
			return new CommandParameter("edit");
		}

		public static ICommandArgument Edit(string @object)
		{
			return new CommandParameterValue("edit", @object, ' ');
		}

		public static ICommandArgument Show()
		{
			return new CommandParameter("show");
		}

		public static ICommandArgument Show(string @object)
		{
			return new CommandParameterValue("show", @object, ' ');
		}

		public static ICommandArgument Remove()
		{
			return new CommandParameter("remove");
		}

		public static ICommandArgument Remove(string @object)
		{
			return new CommandParameterValue("remove", @object, ' ');
		}

		public static ICommandArgument Prune()
		{
			return new CommandParameter("prune");
		}

		public static ICommandArgument Force()
		{
			return new CommandFlag("--force");
		}

		public static ICommandArgument Stdin()
		{
			return new CommandFlag("--stdin");
		}

		public static ICommandArgument File(string file)
		{
			return new CommandParameterValue("--file", file.SurroundWith("\"", "\""), '=');
		}

		public static ICommandArgument Message(string message)
		{
			return new CommandParameterValue("--message", message.SurroundWith("\"", "\""));
		}

		public static ICommandArgument ReuseMessage(string @object)
		{
			return new CommandParameterValue("--reuse-message", @object, '=');
		}

		public static ICommandArgument ReeditMessage(string @object)
		{
			return new CommandParameterValue("--reedit-message", @object, '=');
		}

		public static ICommandArgument Ref(string @ref)
		{
			return new CommandParameterValue("--ref", @ref, ' ');
		}

		public static ICommandArgument DoNotRemove()
		{
			return new CommandFlag("-n");
		}

		public static ICommandArgument VerboseRemove()
		{
			return new CommandFlag("-v");
		}

		public static ICommandArgument GetRef()
		{
			return new CommandFlag("get-ref");
		}

		public NotesCommand()
			: base("notes")
		{
		}

		public NotesCommand(params ICommandArgument[] args)
			: base("notes", args)
		{
		}

		public NotesCommand(IList<ICommandArgument> args)
			: base("notes", args)
		{
		}
	}
}
