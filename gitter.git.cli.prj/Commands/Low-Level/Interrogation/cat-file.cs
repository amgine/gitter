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

	/// <summary>List references in a local repository.</summary>
	public sealed class CatFileCommand : Command
	{
		/// <summary>Instead of the content, show the object type.</summary>
		public static ICommandArgument ShowType()
		{
			return new CommandFlag("-t");
		}

		/// <summary>Instead of the content, show the object size.</summary>
		public static ICommandArgument ShowSize()
		{
			return new CommandFlag("-s");
		}

		public static ICommandArgument CheckExists()
		{
			return new CommandFlag("-e");
		}

		public static ICommandArgument Pretty()
		{
			return new CommandFlag("-p");
		}

		/// <summary>Initializes a new instance of the <see cref="CatFileCommand"/> class.</summary>
		public CatFileCommand()
			: base("cat-file")
		{
		}

		public CatFileCommand(params ICommandArgument[] args)
			: base("cat-file", args)
		{
		}

		public CatFileCommand(IList<ICommandArgument> args)
			: base("cat-file", args)
		{
		}
	}
}
