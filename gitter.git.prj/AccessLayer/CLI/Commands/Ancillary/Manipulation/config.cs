namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Get and set repository or global options.</summary>
	public sealed class ConfigCommand : Command
	{
		public static CommandArgument List()
		{
			return new CommandArgument("--list");
		}

		public static CommandArgument NullTerminate()
		{
			return new CommandArgument("--null");
		}

		public static CommandArgument Add()
		{
			return new CommandArgument("--add");
		}

		public static CommandArgument Unset()
		{
			return new CommandArgument("--unset");
		}

		public static CommandArgument UnsetAll()
		{
			return new CommandArgument("--unset-all");
		}

		public static CommandArgument Get(string name)
		{
			return new CommandArgument("--get", name, ' ');
		}

		public static CommandArgument GetAll(string name)
		{
			return new CommandArgument("--get-all", name, ' ');
		}

		public static CommandArgument GetRegexp(string nameregex)
		{
			return new CommandArgument("--get-regexp", nameregex, ' ');
		}

		public static CommandArgument Global()
		{
			return new CommandArgument("--global");
		}

		public static CommandArgument System()
		{
			return new CommandArgument("--system");
		}

		public static CommandArgument Int()
		{
			return new CommandArgument("--int");
		}

		public static CommandArgument Path()
		{
			return new CommandArgument("--path");
		}

		public static CommandArgument Edit()
		{
			return new CommandArgument("--edit");
		}

		public static CommandArgument Bool()
		{
			return new CommandArgument("--bool");
		}

		public static CommandArgument BoolOrInt()
		{
			return new CommandArgument("--bool-or-int");
		}

		public static CommandArgument File(string fileName)
		{
			return new CommandArgument("--file", fileName, ' ');
		}

		public static CommandArgument RemoveSection(string name)
		{
			return new CommandArgument("--remove-section", name, ' ');
		}

		public static CommandArgument RenameSection(string oldName, string newName)
		{
			return new CommandArgument("--rename-section", oldName + " " + newName, ' ');
		}

		public ConfigCommand()
			: base("config")
		{
		}

		public ConfigCommand(params CommandArgument[] args)
			: base("config", args)
		{
		}

		public ConfigCommand(IList<CommandArgument> args)
			: base("config", args)
		{
		}
	}
}
