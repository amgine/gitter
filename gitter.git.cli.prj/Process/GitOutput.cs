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
	using System.Collections.Generic;
	using System.Text;
	using System.Diagnostics;

	/// <summary>Text output of git.exe.</summary>
	internal sealed class GitOutput
	{
		#region Data

		private readonly string _output;
		private readonly string _error;
		private readonly int _exitCode;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="GitOutput"/>.</summary>
		/// <param name="output">stdout.</param>
		/// <param name="error">stderr.</param>
		/// <param name="exitCode">Process exit code.</param>
		public GitOutput(string output, string error, int exitCode)
		{
			_output   = output;
			_error    = error;
			_exitCode = exitCode;
		}

		#endregion

		#region Properties

		/// <summary>stdout.</summary>
		public string Output => _output;

		/// <summary>stderr.</summary>
		public string Error => _error;

		/// <summary>Process exit code.</summary>
		public int ExitCode => _exitCode;

		#endregion

		#region Methods

		/// <summary>Throw <see cref="GitException"/>.</summary>
		[DebuggerHidden]
		public void Throw()
		{
			throw new GitException(_error.Length == 0 ? _output : _error);
		}

		/// <summary>Throw <see cref="GitException"/> if stderr output is present.</summary>
		[DebuggerHidden]
		public void ThrowOnError()
		{
			if(_error.Length != 0)
			{
				throw new GitException(_error);
			}
		}

		/// <summary>Throw <see cref="GitException"/> if process exit code is non-zero.</summary>
		[DebuggerHidden]
		public void ThrowOnBadReturnCode()
		{
			if(_exitCode != 0)
			{
				throw new GitException(_error.Length == 0 ? _output : _error);
			}
		}

		#endregion

		#region Overrides

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="GitOutput"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="GitOutput"/>.</returns>
		public override string ToString() => _output;

		#endregion
	}
}
