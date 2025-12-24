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

using System;
using System.Collections.Generic;

public abstract class CommandBuilderBase(string commandName) : ICommandBuilder
{
	private List<ICommandArgument>? _args;
	private List<ICommandArgument>? _options;

	public string CommandName { get; } = commandName;

	private static void Add<T>(ref List<T>? list, T value)
		=> (list ??= []).Add(value);

	public void AddArgument(ICommandArgument argument)
		=> Add(ref _args, argument);

	public void AddArgument(string parameter)
		=> AddArgument(new CommandParameter(parameter));

	public void AddOption(ICommandArgument option)
		=> Add(ref _options, option);

	public void AddOption(string option)
		=> AddOption(new CommandParameter(option));

	public Command Build() => new(_options, CommandName, _args);
}
