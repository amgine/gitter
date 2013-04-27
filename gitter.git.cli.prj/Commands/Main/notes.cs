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
		public static CommandArgument List()
		{
			return new CommandArgument("list");
		}

		public static CommandArgument List(string @object)
		{
			return new CommandArgument("list", @object, ' ');
		}

		public static CommandArgument Add()
		{
			return new CommandArgument("add");
		}

		public static CommandArgument Copy()
		{
			return new CommandArgument("copy");
		}

		public static CommandArgument Append()
		{
			return new CommandArgument("copy");
		}

		public static CommandArgument Edit()
		{
			return new CommandArgument("edit");
		}

		public static CommandArgument Edit(string @object)
		{
			return new CommandArgument("edit", @object, ' ');
		}

		public static CommandArgument Show()
		{
			return new CommandArgument("show");
		}

		public static CommandArgument Show(string @object)
		{
			return new CommandArgument("show", @object, ' ');
		}

		public static CommandArgument Remove()
		{
			return new CommandArgument("remove");
		}

		public static CommandArgument Remove(string @object)
		{
			return new CommandArgument("remove", @object, ' ');
		}

		public static CommandArgument Prune()
		{
			return new CommandArgument("prune");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("--force");
		}

		public static CommandArgument Stdin()
		{
			return new CommandArgument("--stdin");
		}

		public static CommandArgument File(string file)
		{
			return new CommandArgument("--file", file.SurroundWith("\"", "\""), '=');
		}

		public static CommandArgument Message(string message)
		{
			return new CommandArgument("--message", message.SurroundWith("\"", "\""));
		}

		public static CommandArgument ReuseMessage(string @object)
		{
			return new CommandArgument("--reuse-message", @object, '=');
		}

		public static CommandArgument ReeditMessage(string @object)
		{
			return new CommandArgument("--reedit-message", @object, '=');
		}

		public static CommandArgument Ref(string @ref)
		{
			return new CommandArgument("--ref", @ref, ' ');
		}

		public static CommandArgument DoNotRemove()
		{
			return new CommandArgument("-n");
		}

		public static CommandArgument VerboseRemove()
		{
			return new CommandArgument("-v");
		}

		public static CommandArgument GetRef()
		{
			return new CommandArgument("get-ref");
		}

		public NotesCommand()
			: base("notes")
		{
		}

		public NotesCommand(params CommandArgument[] args)
			: base("notes", args)
		{
		}

		public NotesCommand(IList<CommandArgument> args)
			: base("notes", args)
		{
		}
	}
}
