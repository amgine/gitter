namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Globalization;
	using System.Collections.Generic;

	sealed class RepackCommand : Command
	{
		public static CommandArgument Window(int window)
		{
			return new CommandArgument("--window", window.ToString(CultureInfo.InvariantCulture), '=');
		}

		public static CommandArgument WindowMemory(int windowMemory)
		{
			return new CommandArgument("--window-memory", windowMemory.ToString(CultureInfo.InvariantCulture), '=');
		}

		public static CommandArgument Depth(int depth)
		{
			return new CommandArgument("--depth", depth.ToString(CultureInfo.InvariantCulture), '=');
		}

		public static CommandArgument MaxPackSize(int maxPackSize)
		{
			return new CommandArgument("--max-pack-size", maxPackSize.ToString(CultureInfo.InvariantCulture), '=');
		}

		public RepackCommand()
			: base("repack")
		{
		}

		public RepackCommand(params CommandArgument[] args)
			: base("repack", args)
		{
		}

		public RepackCommand(IEnumerable<CommandArgument> args)
			: base("repack", args)
		{
		}
	}
}
