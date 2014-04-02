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

	/// <summary>Apply a series of patches from a mailbox.</summary>
	public sealed class AmCommand : Command
	{
		/// <summary>Add a Signed-off-by: line to the commit message, using the committer identity of yourself.</summary>
		public static ICommandArgument SignOff()
		{
			return CommandFlag.SignOff();
		}

		/// <summary>Pass -k flag to git-mailinfo (see git-mailinfo(1)).</summary>
		public static ICommandArgument Keep()
		{
			return new CommandFlag("--keep");
		}

		/// <summary>Be quiet. Only print error messages.</summary>
		public static ICommandArgument Quiet()
		{
			return CommandFlag.Quiet();
		}

		/// <summary>
		///	Pass -u flag to git-mailinfo (see git-mailinfo(1)). The proposed commit log message taken from the e-mail
		///	is re-coded into UTF-8 encoding (configuration variable i18n.commitencoding can be used to specify project's
		///	preferred encoding if it is not UTF-8). 
		/// </summary>
		public static ICommandArgument Utf8()
		{
			return new CommandFlag("--utf8");
		}

		/// <summary>
		///	Pass -n flag to git-mailinfo (see git-mailinfo(1)).
		/// </summary>
		public static ICommandArgument NoUtf8()
		{
			return new CommandFlag("--no-utf8");
		}

		/// <summary>
		///	When the patch does not apply cleanly, fall back on 3-way merge if the patch records the identity of blobs it
		///	is supposed to apply to and we have those blobs available locally.
		/// </summary>
		public static ICommandArgument FallbackOn3WayMerge()
		{
			return new CommandFlag("--3way");
		}

		/// <summary>Run interactively.</summary>
		public static ICommandArgument Interactive()
		{
			return new CommandFlag("--interactive");
		}

		public AmCommand()
			: base("am")
		{
		}

		public AmCommand(params ICommandArgument[] args)
			: base("am", args)
		{
		}

		public AmCommand(IList<ICommandArgument> args)
			: base("am", args)
		{
		}
	}
}