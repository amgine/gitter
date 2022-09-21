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

#nullable enable

namespace gitter.Git.AccessLayer.CLI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/// <summary>Input data for git.exe.</summary>
internal sealed class GitInput
{
	public GitInput(Command command, Encoding? encoding = null,
		IReadOnlyDictionary<string, string>? environment = null)
		: this(string.Empty, command, encoding, environment)
	{
	}

	public GitInput(string workingDirectory, Command command, Encoding? encoding = null,
		IReadOnlyDictionary<string, string>? environment = null)
	{
		WorkingDirectory = workingDirectory ?? string.Empty;
		Command          = command;
		Encoding         = encoding ?? GitProcess.DefaultEncoding;
		Environment      = environment;
	}

	public string WorkingDirectory { get; }

	public Encoding Encoding { get; }

	public Command Command { get; }

	public IReadOnlyDictionary<string, string>? Environment { get; }

	public void Configure(ProcessStartInfo processStartInfo)
	{
		Verify.Argument.IsNotNull(processStartInfo);

		if(!string.IsNullOrEmpty(WorkingDirectory))
		{
			processStartInfo.WorkingDirectory = WorkingDirectory;
		}
		processStartInfo.Arguments              = GetArguments();
		processStartInfo.StandardOutputEncoding = Encoding;
		processStartInfo.StandardErrorEncoding  = Encoding;
		if(Environment is { Count: not 0 })
		{
			foreach(var opt in Environment)
			{
				processStartInfo.EnvironmentVariables[opt.Key] = opt.Value;
			}
		}
	}

	public string GetArguments() => Command?.ToString() ?? string.Empty;
}
