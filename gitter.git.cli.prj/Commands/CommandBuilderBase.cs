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

#nullable enable

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;

public abstract class CommandBuilderBase
{
	private List<ICommandArgument>? _args;
	private List<ICommandArgument>? _options;

	protected CommandBuilderBase(string commandName)
	{
		Assert.IsNeitherNullNorWhitespace(commandName);

		CommandName = commandName;
	}

	public string CommandName { get; }

	public void AddArgument(string parameter)
	{
		AddArgument(new CommandParameter(parameter));
	}

	public void AddArgument(ICommandArgument argument)
	{
		Assert.IsNotNull(argument);

		_args ??= new List<ICommandArgument>();
		_args.Add(argument);
	}

	public void AddOption(string option)
	{
		Assert.IsNotNull(option);

		AddOption(new CommandParameter(option));
	}

	public void AddOption(ICommandArgument option)
	{
		Assert.IsNotNull(option);

		_options ??= new List<ICommandArgument>();
		_options.Add(option);
	}

	public Command Build() => new(_options, CommandName, _args);
}
