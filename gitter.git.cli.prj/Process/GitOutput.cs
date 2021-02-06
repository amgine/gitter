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

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Diagnostics;

	/// <summary>Text output of git.exe.</summary>
	internal sealed class GitOutput
	{
		/// <summary>Create <see cref="GitOutput"/>.</summary>
		/// <param name="output">stdout.</param>
		/// <param name="error">stderr.</param>
		/// <param name="exitCode">Process exit code.</param>
		public GitOutput(string output, string error, int exitCode)
		{
			Output   = output;
			Error    = error;
			ExitCode = exitCode;
		}

		/// <summary>stdout.</summary>
		public string Output { get; }

		/// <summary>stderr.</summary>
		public string Error { get; }

		/// <summary>Process exit code.</summary>
		public int ExitCode { get; }

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
			if(ExitCode != 0)
			{
				throw new GitException(Error.Length == 0 ? Output : Error);
			}
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="GitOutput"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="GitOutput"/>.</returns>
		public override string ToString() => Output;
	}
}
