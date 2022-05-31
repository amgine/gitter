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

namespace gitter.Git;

using System;
using System.Collections.Generic;

public static class GitUtils
{
	public static bool IsValidSHA1(string hash)
	{
		if(hash == null) return false;
		if(hash.Length != Hash.HexStringLength) return false;
		for(int i = 0; i < hash.Length; ++i)
		{
			if(!Uri.IsHexDigit(hash[i])) return false;
		}
		return true;
	}

	public static bool IsValidPartialSHA1(string hash)
	{
		if(hash == null) return false;
		if(hash.Length > 40) return false;
		for(int i = 0; i < hash.Length; ++i)
		{
			if(!Uri.IsHexDigit(hash[i])) return false;
		}
		return true;
	}

	private static string[] RemoveTrailingEmptyStringElements(string[] elements)
	{
		var list = new List<string>(elements.Length);
		for(int i = elements.Length - 1; i > -1; i--)
		{
			if(!(elements[i] == string.Empty))
			{
				for(int j = 0; j <= i; ++j)
				{
					list.Add(elements[j]);
				}
				break;
			}
		}
		return list.ToArray();
	}

	public static string GetHumanishName(string url)
	{
		if(string.IsNullOrEmpty(url))
		{
			throw new InvalidOperationException("Path is either null or empty.");
		}
		string[] elements = url.Split(new char[] { '/' });
		if(elements.Length == 0)
		{
			throw new InvalidOperationException();
		}
		string[] strArray2 = RemoveTrailingEmptyStringElements(elements);
		if(strArray2.Length == 0)
		{
			throw new InvalidOperationException();
		}
		string str = strArray2[strArray2.Length - 1];
		if(".git".Equals(str))
		{
			return strArray2[strArray2.Length - 2];
		}
		if(str.EndsWith(".git"))
		{
			str = str.Substring(0, str.Length - ".git".Length);
		}
		int p = str.LastIndexOf(':');
		if(p != -1) str = str.Substring(p + 1);
		return str;
	}
}
