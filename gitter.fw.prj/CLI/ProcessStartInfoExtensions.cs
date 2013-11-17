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

namespace gitter.Framework.CLI
{
	using System.Diagnostics;
	using System.Collections.Specialized;

	public static class ProcessStartInfoExtensions
	{
		public static void EnsureEnvironmentVariableExists(this ProcessStartInfo psi, string variable, string value, bool resetIfExists = false)
		{
			Verify.Argument.IsNotNull(psi, "psi");
			Verify.Argument.IsNeitherNullNorWhitespace(variable, "variable");

			EnsureEnvironmentVariableExists(psi.EnvironmentVariables, variable, value, resetIfExists);
		}

		private static void EnsureEnvironmentVariableExists(StringDictionary dictionary, string variable, string value, bool resetIfExists = false)
		{
			if(resetIfExists)
			{
				dictionary[variable] = value;
			}
			else
			{
				if(!dictionary.ContainsKey(variable))
				{
					dictionary.Add(variable, value);
				}
			}
		}
	}
}
