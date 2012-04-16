namespace gitter.Git.AccessLayer.CLI
{
	using System.Collections.Generic;

	/// <summary>Clone a repository into a new directory.</summary>
	/// <remarks>
	///	git clone [--template=&lt;template_directory&gt;] [-l] [-s] [--no-hardlinks]
	///	[-q] [-n] [--bare] [--mirror] [-o &lt;name&gt;] [-b &lt;name&gt;] [-u &lt;upload-pack&gt;]
	///	[--reference &lt;repository&gt;] [--depth &lt;depth&gt;] [--recursive] [--] &lt;repository&gt; [&lt;directory&gt;]
	/// </remarks>
	public sealed class CloneCommand : Command
	{
		/// <summary>
		/// Make a bare GIT repository. That is, instead of creating directory and placing the administrative files in 
		/// directory/.git, make the directory itself the $GIT_DIR. This obviously implies the -n  because there is nowhere
		/// to check out the working tree. Also the branch heads at the remote are copied directly to corresponding local branch
		/// heads, without mapping them to refs/remotes/origin/. When this option is used, neither remote-tracking branches nor
		/// the related configuration variables are created.
		/// </summary>
		public static CommandArgument Bare()
		{
			return new CommandArgument("--bare");
		}

		public static CommandArgument NoCheckout()
		{
			return new CommandArgument("--no-checkout");
		}

		/// <summary>Instead of using the remote name origin to keep track of the upstream repository, use <paramref name="name"/>.</summary>
		public static CommandArgument Origin(string name)
		{
			return new CommandArgument("--origin", name, ' ');
		}

		/// <summary>
		///	Create a shallow clone with a history truncated to the specified number of revisions. A shallow repository has a
		///	number of limitations (you cannot clone or fetch from it, nor push from nor into it), but is adequate if you are
		///	only interested in the recent history of a large project with a long history, and would want to send in fixes as patches.
		/// </summary>
		public static CommandArgument Depth(int depth)
		{
			return new CommandArgument("--depth", depth.ToString(), ' ');
		}

		/// <summary>Display the progressbar, even in case the standard output is not a terminal.</summary>
		public static CommandArgument Verbose()
		{
			return CommandArgument.Verbose();
		}

		/// <summary>Set up a mirror of the remote repository. This implies --bare.</summary>
		public static CommandArgument Mirror()
		{
			return new CommandArgument("--mirror");
		}

		public static CommandArgument Recursive()
		{
			return new CommandArgument("--recursive");
		}

		public static CommandArgument Progress()
		{
			return new CommandArgument("--progress");
		}

		public static CommandArgument Template(string template)
		{
			return new CommandArgument("--template", template.AssureDoubleQuotes(), '=');
		}

		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public CloneCommand()
			: base("clone")
		{
		}

		public CloneCommand(params CommandArgument[] args)
			: base("clone", args)
		{
		}

		public CloneCommand(IList<CommandArgument> args)
			: base("clone", args)
		{
		}
	}
}
