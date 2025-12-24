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

using System;
using System.Diagnostics;

using gitter.Framework.CLI;

/// <summary>Executes git.exe.</summary>
/// <param name="exeFileName">Path to exe file.</param>
internal sealed class GitProcessExecutor(string exeFileName)
	: ProcessExecutor<GitInput>(exeFileName)
{
	/// <inheritdoc/>
	protected override ProcessStartInfo InitializeStartInfo(GitInput input)
	{
		Assert.IsNotNull(input);

		var psi = new ProcessStartInfo(ExeFileName)
		{
			WindowStyle            = ProcessWindowStyle.Hidden,
			UseShellExecute        = false,
			RedirectStandardInput  = true,
			RedirectStandardOutput = true,
			RedirectStandardError  = true,
			LoadUserProfile        = true,
			ErrorDialog            = false,
			CreateNoWindow         = true,
		};
		GitProcess.SetCriticalEnvironmentVariables(psi);
		input.Configure(psi);
		return psi;
	}
}
