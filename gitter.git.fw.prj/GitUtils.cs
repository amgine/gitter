namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	public static class GitUtils
	{
		public static bool IsValidSHA1(string hash)
		{
			if(hash == null) return false;
			if(hash.Length != 40) return false;
			for(int i = 0; i < 40; ++i)
			{
				if(!hash[i].IsHexDigit()) return false;
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
}
