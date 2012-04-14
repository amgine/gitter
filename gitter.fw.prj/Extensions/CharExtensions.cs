namespace gitter
{
	/// <summary>Extension methods for <see cref="System.Char"/>.</summary>
	public static class CharExtensions
	{
		/// <summary>Determines whether this char is hex digit.</summary>
		/// <param name="c">Character.</param>
		/// <returns><c>true</c> if specified char is hex digit; otherwise, <c>false</c>.</returns>
		public static bool IsHexDigit(this char c)
		{
			if(char.IsDigit(c)) return true;
			if(c == 'a') return true;
			if(c == 'b') return true;
			if(c == 'c') return true;
			if(c == 'd') return true;
			if(c == 'e') return true;
			if(c == 'f') return true;
			if(c == 'A') return true;
			if(c == 'B') return true;
			if(c == 'C') return true;
			if(c == 'D') return true;
			if(c == 'E') return true;
			if(c == 'F') return true;
			return false;
		}

		/// <summary>Determines whether this char is oct digit.</summary>
		/// <param name="c">Character.</param>
		/// <returns><c>true</c> if specified char is oct digit; otherwise, <c>false</c>.</returns>
		public static bool IsOctDigit(this char c)
		{
			int digit = c - '0';
			return digit >= 0 && digit < 8;
		}
	}
}
