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

/// <summary>Text output of git.exe.</summary>
/// <param name="Output">stdout.</param>
/// <param name="Error">stderr.</param>
/// <param name="ExitCode">Process exit code.</param>
internal sealed record class GitOutput(
	string Output,
	string Error,
	int    ExitCode)
{
	/// <summary>Throw <see cref="GitException"/>.</summary>
	[DebuggerHidden]
	public void Throw() => throw new GitException(Error.Length == 0 ? Output : Error);

	/// <summary>Throw <see cref="GitException"/> if stderr output is present.</summary>
	[DebuggerHidden]
	public void ThrowOnError()
	{
		if(Error.Length != 0)
		{
			throw new GitException(Error);
		}
	}

	/// <summary>Throw <see cref="GitException"/> if process exit code is non-zero.</summary>
	[DebuggerHidden]
	public void ThrowOnBadReturnCode()
	{
		if(ExitCode != 0) Throw();
	}

	/// <inheritdoc/>
	public override string ToString() => Output;
}
