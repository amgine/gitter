namespace gitter
{
	/// <summary>Extension methods for <see cref="System.Char"/>.</summary>
	public static class CharExtensions
	{
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
