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

namespace gitter.Updater;

using System;
using System.Collections.Generic;

public sealed class CommandLine
{
	private readonly Dictionary<string, CommandLineParameter> _parameters = [];

	public CommandLine()
	{
		var args = Environment.GetCommandLineArgs();
		for(int i = 1; i < args.Length; ++i)
		{
			var arg = args[i].Trim('"');
			if(arg.Length > 1 && arg.StartsWith("/"))
			{
				var colonIndex = arg.IndexOf(':');
				if(colonIndex != -1)
				{
					if(colonIndex > 1)
					{
						string value;
						var name = arg.Substring(1, colonIndex - 1);
						if(colonIndex < arg.Length - 1)
						{
							value = arg.Substring(colonIndex + 1);
						}
						else
						{
							value = string.Empty;
						}
						_parameters[name] = new CommandLineParameter(name, value);
					}
				}
				else
				{
					var name = arg.Substring(1);
					_parameters[name] = new CommandLineParameter(name, null);
				}
			}
		}
	}

	public string? this[string name]
		=> _parameters.TryGetValue(name, out var p)
			? p.Value
			: default;

	public bool IsDefined(string name)
		=> _parameters.ContainsKey(name);
}

public sealed class CommandLineParameter(string name, string? value)
{
	public string Name { get; } = name;

	public string? Value { get; } = value;
}
