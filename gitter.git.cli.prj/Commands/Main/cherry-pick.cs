#region Copyright Notice
/*
* gitter - VCS repository management tool
* Copyright (C) 2019  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.AccessLayer.CLI;

using System.Globalization;
using System.Collections.Generic;

/// <summary>Apply the change introduced by an existing commit.</summary>
public sealed class CherryPickCommand : Command
{
	public static class KnownArguments
	{
		public static ICommandArgument Edit { get; } = new CommandFlag("--edit");

		public static ICommandArgument Mainline(int number) => new CommandParameterValue("--mainline", number.ToString(CultureInfo.InvariantCulture), ' ');

		public static ICommandArgument NoCommit { get; } = new CommandFlag("--no-commit");

		public static ICommandArgument SignOff { get; } = new CommandFlag("--signoff");

		public static ICommandArgument FastForward { get; } = new CommandFlag("--ff");

		public static ICommandArgument AllowEmpty { get; } = new CommandFlag("--allow-empty");

		public static ICommandArgument AllowEmptyMessage { get; } = new CommandFlag("--allow-empty-message");

		public static ICommandArgument KeepRedundantCommits { get; } = new CommandFlag("--keep-redundant-commits");

		public static ICommandArgument Continue { get; } = new CommandFlag("--continue");

		public static ICommandArgument Quit { get; } = new CommandFlag("--quit");

		public static ICommandArgument Abort { get; } = new CommandFlag("--abort");
	}

	public class Builder : CommandBuilderBase
	{
		public Builder()
			: base("cherry-pick")
		{
		}

		public void Edit() => AddArgument(KnownArguments.Edit);

		public void Mainline(int number) => AddArgument(KnownArguments.Mainline(number));

		public void NoCommit() => AddArgument(KnownArguments.NoCommit);

		public void SignOff() => AddArgument(KnownArguments.SignOff);

		public void FastForward() => AddArgument(KnownArguments.SignOff);

		public void AllowEmpty() => AddArgument(KnownArguments.AllowEmpty);

		public void AllowEmptyMessage() => AddArgument(KnownArguments.AllowEmptyMessage);

		public void KeepRedundantCommits() => AddArgument(KnownArguments.KeepRedundantCommits);

		public void Continue() => AddArgument(KnownArguments.Continue);

		public void Quit() => AddArgument(KnownArguments.Quit);

		public void Abort() => AddArgument(KnownArguments.Abort);
	}

	public CherryPickCommand()
		: base("cherry-pick")
	{
	}

	public CherryPickCommand(params ICommandArgument[] args)
		: base("cherry-pick", args)
	{
	}

	public CherryPickCommand(IList<ICommandArgument> args)
		: base("cherry-pick", args)
	{
	}
}
