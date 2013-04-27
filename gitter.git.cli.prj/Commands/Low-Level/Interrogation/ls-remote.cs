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

	/// <summary>List references in a remote repository.</summary>
	public sealed class LsRemoteCommand : Command
	{
		public static LsRemoteCommand FormatGetBranchesCommand(string remote)
		{
			return new LsRemoteCommand(
				Heads(),
				new CommandArgument(remote),
				new CommandArgument("/refs/heads/*"));
		}

		public static CommandArgument Heads()
		{
			return new CommandArgument("--heads");
		}

		public static CommandArgument Tags()
		{
			return new CommandArgument("--tags");
		}

		public static CommandArgument UploadPack(string path)
		{
			return new CommandArgument("--upload-pack", path, '=');
		}

		public LsRemoteCommand()
			: base("ls-remote")
		{
		}

		public LsRemoteCommand(params CommandArgument[] args)
			: base("ls-remote", args)
		{
		}

		public LsRemoteCommand(IList<CommandArgument> args)
			: base("ls-remote", args)
		{
		}
	}
}
