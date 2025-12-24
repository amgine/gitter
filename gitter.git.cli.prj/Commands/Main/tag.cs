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

namespace gitter.Git.AccessLayer.CLI;

using System.Collections.Generic;

/// <summary>Create, list, delete or verify a tag object signed with GPG.</summary>
public sealed class TagCommand : Command
{
	const string TagCommandName = "tag";

	public static ICommandArgument Annotate()
		=> new CommandFlag("-a");

	public static ICommandArgument SignByEmail()
		=> new CommandFlag("-s");

	public static ICommandArgument Message(string message)
		=> new CommandParameterQuotedValue("-m", message, ' ');

	public static ICommandArgument MessageFromFile(string filename)
		=> new CommandParameterPathValue("-F", filename, ' ');

	public static ICommandArgument SignByKey(string keyid)
		=> new CommandParameterValue("-u", keyid, ' ');

	public static ICommandArgument Verify()
		=> new CommandFlag("-v");

	public static ICommandArgument List()
		=> new CommandFlag("-l");

	public static ICommandArgument Force()
		=> new CommandFlag("-f");

	public static ICommandArgument List(string pattern)
		=> new CommandParameterValue("-l", pattern, ' ');

	public static ICommandArgument ReplaceExisting()
		=> new CommandFlag("-f");

	public static ICommandArgument Delete()
		=> new CommandFlag("-d");

	public TagCommand()
		: base(TagCommandName)
	{
	}

	public TagCommand(params ICommandArgument[] args)
		: base(TagCommandName, args)
	{
	}

	public TagCommand(IList<ICommandArgument> args)
		: base(TagCommandName, args)
	{
	}
}
