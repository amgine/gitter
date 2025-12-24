#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using gitter.Framework;

public interface ICommandBuilder
{
	string CommandName { get; }

	void AddArgument(ICommandArgument argument);

	void AddOption(ICommandArgument option);

	Command Build();
}

public interface ICommandBuilderSpecifyRefs : ICommandBuilder
{
}

public interface ICommandBuilderSpecifyPaths : ICommandBuilder
{
}

public interface IDiffCommandBuilder
	: ICommandBuilder
	, ICommandBuilderSpecifyPaths
{
	void NullTerminate();
}

public static class CommandBuilderExtensions
{
	public static void SpecifyReference<T>(this T buider, Sha1Hash hash)
		where T : ICommandBuilderSpecifyRefs => buider.AddArgument(new CommandParameterHash(hash));

	public static void SpecifyReference<T>(this T buider, string reference)
		where T : ICommandBuilderSpecifyRefs => buider.AddArgument(new CommandParameter(reference));

	public static void SpecifyOptionalReference<T>(this T buider, string? reference)
		where T : ICommandBuilderSpecifyRefs
	{
		if(reference is not { Length: not 0 }) return;
		SpecifyReference(buider, reference);
	}

	public static void SpecifyReferences<T>(this T buider, Many<string> references)
		where T : ICommandBuilderSpecifyRefs
	{
		foreach(var reference in references)
		{
			SpecifyReference(buider, reference);
		}
	}

	public static void SpecifyPath<T>(this T buider, string path)
		where T : ICommandBuilderSpecifyPaths => buider.AddArgument(new PathCommandArgument(path));

	public static void NoMoreOptions<T>(this T buider)
		where T : ICommandBuilderSpecifyPaths => buider.AddArgument(CommandFlag.NoMoreOptions);

	public static bool SpecifyPaths<T>(this T buider, Many<string> paths)
		where T : ICommandBuilderSpecifyPaths
	{
		if(paths.IsEmpty) return false;
		NoMoreOptions(buider);
		foreach(var path in paths)
		{
			SpecifyPath(buider, path);
		}
		return true;
	}
}
