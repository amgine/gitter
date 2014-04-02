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
		public static ICommandArgument Clear()
		{
			return new CommandParameter("clear");
		}

		public static ICommandArgument Forget(string pathspec)
		{
			return new CommandParameterValue("forget", pathspec, ' ');
		}

		public static ICommandArgument Diff()
		{
			return new CommandParameter("diff");
		}

		public static ICommandArgument Remaining()
		{
			return new CommandParameter("remaining");
		}

		public static ICommandArgument Status()
		{
			return new CommandParameter("status");
		}

		public static ICommandArgument Gc()
		{
			return new CommandParameter("gc");
		}

		public RerereCommand()
			: base("rerere")
		{
		}

		public RerereCommand(params ICommandArgument[] args)
			: base("rerere", args)
		{
		}

		public RerereCommand(IEnumerable<ICommandArgument> args)
			: base("rerere", args)
		{
		}
	}
}
