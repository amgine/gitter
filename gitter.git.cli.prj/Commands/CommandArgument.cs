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
	using System;
	using System.Text;

	/// <summary>Command argument.</summary>
	public class CommandArgument
	{
		#region Data

		private readonly string _name;
		private readonly char _separator;
		private string _value;

		#endregion

		#region Static

		public static CommandArgument DryRun()
		{
			return new CommandArgument("--dry-run");
		}

		public static CommandArgument Verbose()
		{
			return new CommandArgument("--verbose");
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument SignOff()
		{
			return new CommandArgument("--signoff");
		}

		public static CommandArgument Interactive()
		{
			return new CommandArgument("--interactive");
		}

		/// <summary>Do not interpret any more arguments as options.</summary>
		public static CommandArgument NoMoreOptions()
		{
			return new CommandArgument("--");
		}

		#endregion

		#region .ctor

		public CommandArgument(string name)
		{
			_name = name;
			_separator = '=';
		}

		public CommandArgument(string name, string value)
			: this(name)
		{
			_value = value;
		}

		public CommandArgument(string name, string value, char separator)
			: this(name, value)
		{
			_separator = separator;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		public char Separator
		{
			get { return _separator; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		#endregion

		#region Methods

		public string GetArgument()
		{
			if(string.IsNullOrEmpty(_value)) return _name;
			return _name + _separator + _value;
		}

		public void GetArgument(StringBuilder stringBuilder)
		{
			Verify.Argument.IsNotNull(stringBuilder, "stringBuilder");

			stringBuilder.Append(_name);
			if(!string.IsNullOrEmpty(_value))
			{
				stringBuilder.Append(_separator);
				stringBuilder.Append(_value);
			}
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return GetArgument();
		}

		#endregion
	}

	public class PathCommandArgument : CommandArgument
	{
		public PathCommandArgument(string path)
			: base(path.AssureDoubleQuotes())
		{
		}
	}
}
