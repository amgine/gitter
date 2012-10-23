namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Diagnostics;
	using System.IO;

	/// <summary>Receives output from stderr/stdout.</summary>
	internal interface IOutputReceiver
	{
		void Initialize(Process process, StreamReader sr);

		void Close();
	}
}
