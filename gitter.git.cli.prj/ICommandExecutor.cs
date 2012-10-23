namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;

	/// <summary>Git command line executor.</summary>
	internal interface ICommandExecutor
	{
		GitOutput ExecCommand(Command command);

		GitOutput ExecCommand(Command command, Encoding encoding);

		GitAsync ExecAsync(Command command);

		GitAsync ExecAsync(Command command, Encoding encoding);
	}
}
