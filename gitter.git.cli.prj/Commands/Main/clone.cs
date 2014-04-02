#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
		public static ICommandArgument Bare()
		{
			return new CommandFlag("--bare");
		}

		public static ICommandArgument NoCheckout()
		{
			return new CommandFlag("--no-checkout");
		}

		/// <summary>Instead of using the remote name origin to keep track of the upstream repository, use <paramref name="name"/>.</summary>
		public static ICommandArgument Origin(string name)
		{
			return new CommandParameterValue("--origin", name, ' ');
		}

		/// <summary>
		///	Create a shallow clone with a history truncated to the specified number of revisions. A shallow repository has a
		///	number of limitations (you cannot clone or fetch from it, nor push from nor into it), but is adequate if you are
		///	only interested in the recent history of a large project with a long history, and would want to send in fixes as patches.
		/// </summary>
		public static ICommandArgument Depth(int depth)
		{
			return new CommandParameterValue("--depth", depth.ToString(), ' ');
		}

		/// <summary>Display the progressbar, even in case the standard output is not a terminal.</summary>
		public static ICommandArgument Verbose()
		{
			return CommandFlag.Verbose();
		}

		/// <summary>Set up a mirror of the remote repository. This implies --bare.</summary>
		public static ICommandArgument Mirror()
		{
			return new CommandFlag("--mirror");
		}

		public static ICommandArgument Recursive()
		{
			return new CommandFlag("--recursive");
		}

		public static ICommandArgument Progress()
		{
			return new CommandFlag("--progress");
		}

		public static ICommandArgument Template(string template)
		{
			return new CommandParameterValue("--template", template.AssureDoubleQuotes(), '=');
		}

		public static ICommandArgument NoMoreOptions()
		{
			return CommandFlag.NoMoreOptions();
		}

		public CloneCommand()
			: base("clone")
		{
		}

		public CloneCommand(params ICommandArgument[] args)
			: base("clone", args)
		{
		}

		public CloneCommand(IList<ICommandArgument> args)
			: base("clone", args)
		{
		}
	}
}
