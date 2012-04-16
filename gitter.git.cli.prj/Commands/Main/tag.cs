namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Create, list, delete or verify a tag object signed with GPG.</summary>
	public sealed class TagCommand : Command
	{
		public static CommandArgument Annotate()
		{
			return new CommandArgument("-a");
		}

		public static CommandArgument SignByEmail()
		{
			return new CommandArgument("-s");
		}

		public static CommandArgument Message(string message)
		{
			return new CommandArgument("-m", "\"" + message + "\"", ' ');
		}

		public static CommandArgument MessageFromFile(string filename)
		{
			return new CommandArgument("-F", filename, ' ');
		}

		public static CommandArgument SignByKey(string keyid)
		{
			return new CommandArgument("-u", keyid, ' ');
		}

		public static CommandArgument Verify()
		{
			return new CommandArgument("-v");
		}

		public static CommandArgument List()
		{
			return new CommandArgument("-l");
		}

		public static CommandArgument Force()
		{
			return new CommandArgument("-f");
		}

		public static CommandArgument List(string pattern)
		{
			return new CommandArgument("-l", pattern, ' ');
		}

		public static CommandArgument ReplaceExisting()
		{
			return new CommandArgument("-f");
		}

		public static CommandArgument Delete()
		{
			return new CommandArgument("-d");
		}

		public TagCommand()
			: base("tag")
		{
		}

		public TagCommand(params CommandArgument[] args)
			: base("tag", args)
		{
		}

		public TagCommand(IList<CommandArgument> args)
			: base("tag", args)
		{
		}
	}
}
