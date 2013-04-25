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
