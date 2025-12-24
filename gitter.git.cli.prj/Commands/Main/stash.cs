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

/// <summary>Stash the changes in a dirty working directory away.</summary>
public sealed class StashCommand : Command
{
	const string StashCommandName = "stash";

	public static class KnownArguments
	{
		public static ICommandArgument Save { get; }
			= new CommandParameter("save");

		public static ICommandArgument List { get; }
			= new CommandParameter("list");

		public static ICommandArgument Show { get; }
			= new CommandParameter("show");

		public static ICommandArgument Pop { get; }
			= new CommandParameter("pop");

		public static ICommandArgument Apply { get; }
			= new CommandParameter("apply");

		public static ICommandArgument Branch { get; }
			= new CommandParameter("branch");

		public static ICommandArgument Clear { get; }
			= new CommandParameter("clear");

		public static ICommandArgument Drop { get; }
			= new CommandParameter("drop");

		public static ICommandArgument Create { get; }
			= new CommandParameter("create");

		public static ICommandArgument NoKeepIndex { get; }
			= new CommandFlag("--no-keep-index");

		public static ICommandArgument KeepIndex { get; }
			= new CommandFlag("--keep-index");

		public static ICommandArgument Index { get; }
			= new CommandFlag("--index");

		public static ICommandArgument IncludeUntracked { get; }
			= new CommandFlag("--include-untracked");

		public static ICommandArgument NoMoreOptions
			=> CommandFlag.NoMoreOptions;

		public static ICommandArgument NullTerminate { get; }
			= new CommandFlag("-z");

		public static ICommandArgument Quiet
			=> CommandFlag.Quiet;
	}

	public sealed class Builder()
		: CommandBuilderBase(StashCommandName)
		, IDiffCommandBuilder
	{
		public void Save()
			=> AddArgument(KnownArguments.Save);

		public void List()
			=> AddArgument(KnownArguments.List);

		public void Show()
			=> AddArgument(KnownArguments.Show);

		public void Pop()
			=> AddArgument(KnownArguments.Pop);

		public void Apply()
			=> AddArgument(KnownArguments.Apply);

		public void Branch()
			=> AddArgument(KnownArguments.Branch);

		public void Clear()
			=> AddArgument(KnownArguments.Clear);

		public void Drop()
			=> AddArgument(KnownArguments.Drop);

		public void Create()
			=> AddArgument(KnownArguments.Create);

		public void NoKeepIndex()
			=> AddArgument(KnownArguments.NoKeepIndex);

		public void KeepIndex()
			=> AddArgument(KnownArguments.KeepIndex);

		public void Index()
			=> AddArgument(KnownArguments.Index);

		public void IncludeUntracked()
			=> AddArgument(KnownArguments.IncludeUntracked);

		public void Quiet()
			=> AddArgument(KnownArguments.Quiet);

		public void NullTerminate()
			=> AddArgument(KnownArguments.NullTerminate);

		public void NoMoreOptions()
			=> AddArgument(KnownArguments.NoMoreOptions);
	}

	public StashCommand()
		: base(StashCommandName)
	{
	}

	public StashCommand(params ICommandArgument[] args)
		: base(StashCommandName, args)
	{
	}

	public StashCommand(IList<ICommandArgument> args)
		: base(StashCommandName, args)
	{
	}
}
