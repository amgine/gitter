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

	/// <summary>Reuse recorded resolution of conflicted merges.</summary>
	/// <remarks>You need to set the configuration variable rerere.enabled in order to enable this command.</remarks>
	sealed class RerereCommand : Command
	{
		public static CommandArgument Clear()
		{
			return new CommandArgument("clear");
		}

		public static CommandArgument Forget(string pathspec)
		{
			return new CommandArgument("forget", pathspec, ' ');
		}

		public static CommandArgument Diff()
		{
			return new CommandArgument("diff");
		}

		public static CommandArgument Remaining()
		{
			return new CommandArgument("remaining");
		}

		public static CommandArgument Status()
		{
			return new CommandArgument("status");
		}

		public static CommandArgument Gc()
		{
			return new CommandArgument("gc");
		}

		public RerereCommand()
			: base("rerere")
		{
		}

		public RerereCommand(params CommandArgument[] args)
			: base("rerere", args)
		{
		}

		public RerereCommand(IEnumerable<CommandArgument> args)
			: base("rerere", args)
		{
		}
	}
}
