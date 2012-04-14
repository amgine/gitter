namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Diagnostics;

	/// <summary>Output of git.exe.</summary>
	internal sealed class GitOutput
	{
		#region Data

		private readonly string _output;
		private readonly string _error;
		private readonly int _processExitCode;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="GitOutput"/>.</summary>
		/// <param name="output">stdout.</param>
		/// <param name="error">stderr.</param>
		/// <param name="exitCode">Process exit code.</param>
		public GitOutput(string output, string error, int exitCode)
		{
			_output = output;
			_error = error;
			_processExitCode = exitCode;
		}

		#endregion

		#region Properties

		/// <summary>stdout.</summary>
		public string Output
		{
			get { return _output; }
		}

		/// <summary>stderr.</summary>
		public string Error
		{
			get { return _error; }
		}

		/// <summary>Process exit code.</summary>
		public int ExitCode
		{
			get { return _processExitCode; }
		}

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
			if(_processExitCode != 0)
			{
				throw new GitException(_error.Length == 0 ? _output : _error);
			}
		}

		#endregion

		#region Overrides

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="GitOutput"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="GitOutput"/>.</returns>
		public override string ToString()
		{
			return _output;
		}

		#endregion
	}
}
