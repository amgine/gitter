#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System;
	using System.Text;

	/// <summary>Command flag.</summary>
	public class CommandFlag : ICommandArgument
	{
		#region Data

		private readonly string _name;

		#endregion

		#region Static

		public static ICommandArgument DryRun()
		{
			return new CommandFlag("--dry-run");
		}

		public static ICommandArgument Verbose()
		{
			return new CommandFlag("--verbose");
		}

		public static ICommandArgument Quiet()
		{
			return new CommandFlag("--quiet");
		}

		public static ICommandArgument SignOff()
		{
			return new CommandFlag("--signoff");
		}

		public static ICommandArgument Interactive()
		{
			return new CommandFlag("--interactive");
		}

		/// <summary>Do not interpret any more arguments as options.</summary>
		public static ICommandArgument NoMoreOptions()
		{
			return new CommandFlag("--");
		}

		#endregion

		#region .ctor

		public CommandFlag(string name)
		{
			_name = name;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		#endregion

		#region Methods

		public void ToString(StringBuilder stringBuilder)
		{
			Verify.Argument.IsNotNull(stringBuilder, "stringBuilder");

			stringBuilder.Append(Name);
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return Name;
		}

		#endregion
	}
}
