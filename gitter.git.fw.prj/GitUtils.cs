namespace gitter.Git
{
	using System;

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
	}
}
