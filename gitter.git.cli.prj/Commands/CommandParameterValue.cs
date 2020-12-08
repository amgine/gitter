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
		protected const char DefaultSeparator = '=';

		public CommandParameterValue(string name, string value, char separator = DefaultSeparator)
		{
			Name      = name;
			Separator = separator;
			Value     = value;
		}

		public string Name { get; }

		public char Separator { get; }

		public string Value { get; }

		public void ToString(StringBuilder stringBuilder)
		{
			Verify.Argument.IsNotNull(stringBuilder, nameof(stringBuilder));

			stringBuilder.Append(Name);
			stringBuilder.Append(Separator);
			stringBuilder.Append(Value);
		}

		public override string ToString() => Name + Separator + Value;
	}
}
