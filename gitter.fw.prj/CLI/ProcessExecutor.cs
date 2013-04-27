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

namespace gitter.Framework.CLI
{
	using System;
	using System.Diagnostics;

	/// <summary>Executes process with optional output redirecting.</summary>
	public abstract class ProcessExecutor<TInput>
		where TInput : class
	{
		#region Data

		private readonly string _path;
		private IOutputReceiver _stdOutReceiver;
		private IOutputReceiver _stdErrReceiver;
		private Process _process;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="ProcessExecutor&lt;TInput&gt;"/> class.</summary>
		/// <param name="path">Path to exe file.</param>
		/// <param name="stdOutReceiver">STDOUT receiver (can be null).</param>
		/// <param name="stdErrReceiver">STDERR receiver (can be null).</param>
		public ProcessExecutor(string path, IOutputReceiver stdOutReceiver, IOutputReceiver stdErrReceiver)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");

			_path = path;
			_stdOutReceiver = stdOutReceiver;
			_stdErrReceiver = stdErrReceiver;
		}

		#endregion

		public string FileName
		{
			get { return _path; }
		}

		protected virtual ProcessStartInfo InitializeStartInfo(TInput input)
		{
			return new ProcessStartInfo()
			{
				FileName = _path,
			};
		}

		public int Execute(TInput input)
		{
			BeginExecute(input);
			return EndExecute();
		}

		public void BeginExecute(TInput input)
		{
			Verify.Argument.IsNotNull(input, "input");
			Verify.State.IsFalse(IsStarted);

			_process = Process.Start(InitializeStartInfo(input));
			if(_stdOutReceiver != null)
			{
				_stdOutReceiver.Initialize(_process, _process.StandardOutput);
			}
			if(_stdErrReceiver != null)
			{
				_stdErrReceiver.Initialize(_process, _process.StandardError);
			}
		}

		public int EndExecute()
		{
			Verify.State.IsTrue(IsStarted);

			_process.WaitForExit();
			if(_stdErrReceiver != null)
			{
				_stdErrReceiver.Close();
			}
			if(_stdOutReceiver != null)
			{
				_stdOutReceiver.Close();
			}
			int code = _process.ExitCode;
			_process.Dispose();
			_process = null;
			return code;
		}

		public bool IsStarted
		{
			get { return _process != null; }
		}
	}
}
