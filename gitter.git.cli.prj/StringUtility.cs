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

namespace gitter.Git.AccessLayer
{
	static class StringUtility
	{
		public static bool CheckValue(string data, int pos, string value)
		{
			if(pos + value.Length > data.Length) return false;
			return data.IndexOf(value, pos, value.Length) == pos;
		}

		public static bool CheckValues(string data, int pos, params string[] values)
		{
			for(int i = 0; i < values.Length; ++i)
			{
				if(!CheckValue(data, pos, values[i])) return false;
				pos += values[i].Length;
			}
			return true;
		}

		public static bool CheckValues(string data, params string[] values)
		{
			int pos = 0;
			for(int i = 0; i < values.Length; ++i)
			{
				if(!CheckValue(data, pos, values[i])) return false;
				pos += values[i].Length;
			}
			return pos == data.Length;
		}
	}
}
