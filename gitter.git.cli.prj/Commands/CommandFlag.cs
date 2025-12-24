#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2018  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using System;
using System.Text;

/// <summary>Command flag.</summary>
public record class CommandFlag(string Name, string LongName) : ICommandArgument
{
	public static ICommandArgument DryRun { get; } = new CommandFlag("--dry-run");

	public static ICommandArgument Verbose => CommonArguments.Verbose;

	public static ICommandArgument Quiet => CommonArguments.Quiet;

	public static ICommandArgument SignOff { get; } = new CommandFlag("--signoff");

	public static ICommandArgument Interactive { get; } = new CommandFlag("--interactive");

	/// <summary>Do not interpret any more arguments as options.</summary>
	public static ICommandArgument NoMoreOptions => CommonArguments.NoMoreOptions;

	public CommandFlag(string Name) : this(Name, Name)
	{
	}

	/// <inheritdoc/>
	public void ToString(StringBuilder stringBuilder)
	{
		Verify.Argument.IsNotNull(stringBuilder);

		stringBuilder.Append(Name);
	}

	/// <inheritdoc/>
	public override string ToString() => Name;
}
