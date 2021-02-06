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
			=> new CommandParameter("list");

		public static ICommandArgument List(string @object)
			=> new CommandParameterValue("list", @object, ' ');

		public static ICommandArgument Add()
			=> new CommandParameter("add");

		public static ICommandArgument Copy()
			=> new CommandParameter("copy");

		public static ICommandArgument Append()
			=> new CommandParameter("append");

		public static ICommandArgument Edit()
			=> new CommandParameter("edit");

		public static ICommandArgument Edit(string @object)
			=> new CommandParameterValue("edit", @object, ' ');

		public static ICommandArgument Show()
			=> new CommandParameter("show");

		public static ICommandArgument Show(string @object)
			=> new CommandParameterValue("show", @object, ' ');

		public static ICommandArgument Remove()
			=> new CommandParameter("remove");

		public static ICommandArgument Remove(string @object)
			=> new CommandParameterValue("remove", @object, ' ');

		public static ICommandArgument Prune()
			=> new CommandParameter("prune");

		public static ICommandArgument Force()
			=> new CommandFlag("--force");

		public static ICommandArgument Stdin()
			=> new CommandFlag("--stdin");

		public static ICommandArgument File(string file)
			=> new CommandParameterValue("--file", file.SurroundWith("\"", "\""), '=');

		public static ICommandArgument Message(string message)
			=> new CommandParameterValue("--message", message.SurroundWith("\"", "\""));

		public static ICommandArgument ReuseMessage(string @object)
			=> new CommandParameterValue("--reuse-message", @object, '=');

		public static ICommandArgument ReeditMessage(string @object)
			=> new CommandParameterValue("--reedit-message", @object, '=');

		public static ICommandArgument Ref(string @ref)
			=> new CommandParameterValue("--ref", @ref, ' ');

		public static ICommandArgument DoNotRemove()
			=> new CommandFlag("-n");

		public static ICommandArgument VerboseRemove()
			=> new CommandFlag("-v");

		public static ICommandArgument GetRef()
			=> new CommandFlag("get-ref");

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
