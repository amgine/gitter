namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;

	/// <summary>Represents an ineractive mode dialog with git.exe.</summary>
	internal sealed class GitInteractive
	{
		private readonly Process _process;

		public GitInteractive(Process process)
		{
			if(process == null) throw new ArgumentNullException("process");
			_process = process;
		}

		public string ReadStdin()
		{
			return _process.StandardOutput.ReadToEnd();
		}

		public string ReadStdErr()
		{
			return _process.StandardError.ReadToEnd();
		}

		public void Write(char data)
		{
			_process.StandardInput.Write(data);
		}

		public void Write(string data)
		{
			_process.StandardInput.Write(data);
		}

		public void WriteLn(string data)
		{
			_process.StandardInput.WriteLine(data);
		}

		public bool HasExited
		{
			get { return _process.HasExited; }
		}

		public int WaitForExit()
		{
			_process.WaitForExit();
			return _process.ExitCode;
		}
	}
}
