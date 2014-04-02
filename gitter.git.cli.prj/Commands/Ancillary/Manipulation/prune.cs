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

	using gitter.Framework;

	/// <summary>Prune all unreachable objects from the object database.</summary>
	sealed class PruneCommand : Command
	{
		public static ICommandArgument DryRun()
		{
			return CommandFlag.DryRun();
		}

		public static ICommandArgument Verbose()
		{
			return CommandFlag.Verbose();
		}

		public static ICommandArgument Expire(DateTime expire)
		{
			return new CommandParameterValue("--expire", Utility.FormatDate(expire, DateFormat.UnixTimestamp), ' ');
		}

		public static ICommandArgument NoMoreOptions()
		{
			return CommandFlag.NoMoreOptions();
		}

		public PruneCommand()
			: base("prune")
		{
		}

		public PruneCommand(params ICommandArgument[] args)
			: base("prune", args)
		{
		}

		public PruneCommand(IList<ICommandArgument> args)
			: base("prune", args)
		{
		}
	}
}
