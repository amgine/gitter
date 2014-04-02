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
	using System.Globalization;
	using System.Collections.Generic;

	sealed class RepackCommand : Command
	{
		public static ICommandArgument Window(int window)
		{
			return new CommandParameterValue("--window", window.ToString(CultureInfo.InvariantCulture), '=');
		}

		public static ICommandArgument WindowMemory(int windowMemory)
		{
			return new CommandParameterValue("--window-memory", windowMemory.ToString(CultureInfo.InvariantCulture), '=');
		}

		public static ICommandArgument Depth(int depth)
		{
			return new CommandParameterValue("--depth", depth.ToString(CultureInfo.InvariantCulture), '=');
		}

		public static ICommandArgument MaxPackSize(int maxPackSize)
		{
			return new CommandParameterValue("--max-pack-size", maxPackSize.ToString(CultureInfo.InvariantCulture), '=');
		}

		public RepackCommand()
			: base("repack")
		{
		}

		public RepackCommand(params ICommandArgument[] args)
			: base("repack", args)
		{
		}

		public RepackCommand(IEnumerable<ICommandArgument> args)
			: base("repack", args)
		{
		}
	}
}
