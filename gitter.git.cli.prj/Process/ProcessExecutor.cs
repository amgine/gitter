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

	using gitter.Framework.CLI;

	/// <summary>Executes git.exe.</summary>
	internal sealed class ProcessExecutor : ProcessExecutor<GitInput>
	{
		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="ProcessExecutor"/> class.</summary>
		/// <param name="path">Path to exe file.</param>
		/// <param name="stdOutReceiver">STDOUT receiver (can be null).</param>
		/// <param name="stdErrReceiver">STDERR receiver (can be null).</param>
		public ProcessExecutor(string path, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
			: base(path, stdOutReceiver, stdErrReceiver)
		{
		}

		#endregion

		protected override ProcessStartInfo InitializeStartInfo(GitInput input)
		{
			var psi = new ProcessStartInfo()
			{
				Arguments				= input.GetArguments(),
				WindowStyle				= ProcessWindowStyle.Hidden,
				UseShellExecute			= false,
				StandardOutputEncoding	= input.Encoding,
				StandardErrorEncoding	= input.Encoding,
				RedirectStandardInput	= true,
				RedirectStandardOutput	= true,
				RedirectStandardError	= true,
				LoadUserProfile			= true,
				FileName				= FileName,
				ErrorDialog				= false,
				CreateNoWindow			= true,
			};
			if(!string.IsNullOrEmpty(input.WorkingDirectory))
			{
				psi.WorkingDirectory = input.WorkingDirectory;
			}
			GitProcess.SetCriticalEnvironmentVariables(psi);
			if(input.Environment != null)
			{
				foreach(var opt in input.Environment)
				{
					psi.EnvironmentVariables[opt.Key] = opt.Value;
				}
			}
			return psi;
		}
	}
}
