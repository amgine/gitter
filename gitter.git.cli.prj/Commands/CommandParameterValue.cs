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

	/// <summary>Command parameter (param-name = value).</summary>
	public class CommandParameterValue : ICommandArgument
	{
		#region Constants

		protected const char DefaultSeparator = '=';

		#endregion

		#region Data

		private readonly string _name;
		private readonly char _separator;
		private string _value;

		#endregion

		#region .ctor

		public CommandParameterValue(string name, string value)
		{
			_name      = name;
			_separator = DefaultSeparator;
			_value     = value;
		}

		public CommandParameterValue(string name, string value, char separator)
		{
			_name      = name;
			_separator = separator;
			_value     = value;
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
		}

		#endregion

		#region Methods

		public void ToString(StringBuilder stringBuilder)
		{
			Verify.Argument.IsNotNull(stringBuilder, "stringBuilder");

			stringBuilder.Append(_name);
			stringBuilder.Append(_separator);
			stringBuilder.Append(_value);
		}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return Name + Separator + Value;
		}

		#endregion
	}
}
