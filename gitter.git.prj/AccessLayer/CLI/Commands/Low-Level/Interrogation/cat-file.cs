namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>List references in a local repository.</summary>
	public sealed class CatFileCommand : Command
	{
		/// <summary>Instead of the content, show the object type.</summary>
		public static CommandArgument ShowType()
		{
			return new CommandArgument("-t");
		}

		/// <summary>Instead of the content, show the object size.</summary>
		public static CommandArgument ShowSize()
		{
			return new CommandArgument("-s");
		}

		public static CommandArgument CheckExists()
		{
			return new CommandArgument("-e");
		}

		public static CommandArgument Pretty()
		{
			return new CommandArgument("-p");
		}

		/// <summary>Initializes a new instance of the <see cref="CatFileCommand"/> class.</summary>
		public CatFileCommand()
			: base("cat-file")
		{
		}

		public CatFileCommand(params CommandArgument[] args)
			: base("cat-file", args)
		{
		}

		public CatFileCommand(IList<CommandArgument> args)
			: base("cat-file", args)
		{
		}
	}
}
