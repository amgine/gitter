namespace gitter
{
	using System;
	using System.Globalization;

	using gitter.Framework;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Extension methods for <see cref="System.String"/>.</summary>
	public static class StringExtensions
	{
		/// <summary>Adds ...</summary>
		public static string AddEllipsis(this string str)
		{
			return string.Format(Resources.StrAddEllipsis, str);
		}

		/// <summary>Adds .</summary>
		public static string AddPeriod(this string str)
		{
			return string.Format(Resources.StrAddPeriod, str);
		}

		/// <summary>Adds ,</summary>
		public static string AddComma(this string str)
		{
			return string.Format(Resources.StrAddComma, str);
		}

		/// <summary>Adds :</summary>
		public static string AddColon(this string str)
		{
			return string.Format(Resources.StrAddColon, str);
		}

		/// <summary>Surrounds with " "</summary>
		public static string SurroundWithDoubleQuotes(this string str)
		{
			return string.Format(Resources.StrSurroundWithDoubleQuotes, str);
		}

		/// <summary>Surrounds with ( )</summary>
		public static string SurroundWithBraces(this string str)
		{
			return "(" + str + ")";
		}

		/// <summary>Surrounds with " " if necessary.</summary>
		public static string AssureDoubleQuotes(this string str)
		{
			int l = str.Length;
			if(l == 0) return "\"\"";
			if(str[0] == '\"' && str[l - 1] == '\"')
				return str;
			else
				return "\"" + str + "\"";
		}

		/// <summary>Surrounds with ' '</summary>
		public static string SurroundWithSingleQuotes(this string str)
		{
			return string.Format(Resources.StrSurroundWithDoubleQuotes, str);
		}

		/// <summary>Surrounds with <paramref name="val"/>.</summary>
		public static string SurroundWith(this string str, string val)
		{
			return val + str + val;
		}

		/// <summary>Surrounds with <paramref name="val"/>.</summary>
		public static string SurroundWith(this string str, char val)
		{
			return val + str + val;
		}

		/// <summary>Surrounds with <paramref name="prefix"/> and <paramref name="postfix"/>.</summary>
		public static string SurroundWith(this string str, string prefix, string postfix)
		{
			return prefix + str + postfix;
		}

		/// <summary>Surrounds with <paramref name="prefix"/> and <paramref name="postfix"/>.</summary>
		public static string SurroundWith(this string str, char prefix, char postfix)
		{
			return prefix + str + postfix;
		}

		public static bool EndsWith(this string str, char value)
		{
			var l = str.Length;
			return l != 0 && str[l - 1] == value;
		}

		public static bool ContainsAnyOf(this string str, char value)
		{
			for(int i = 0; i < str.Length; ++i)
			{
				var c = str[i];
				if(c == value) return true;
			}
			return false;
		}

		public static bool ContainsAnyOf(this string str, char value, params char[] values)
		{
			for(int i = 0; i < str.Length; ++i)
			{
				var c = str[i];
				if(c == value) return true;
				if(values != null)
				{
					for(int j = 0; j < values.Length; ++j)
					{
						if(c == values[j]) return true;
					}
				}
			}
			return false;
		}

		public static bool EndsWithOneOf(this string str, params char[] values)
		{
			var l = str.Length;
			if(l == 0) return values.Length == 0;
			var c = str[l - 1];
			for(int i = 0; i < values.Length; ++i)
			{
				if(values[i] == c) return true;
			}
			return false;
		}

		public static bool StartsWithOneOf(this string str, params char[] values)
		{
			var l = str.Length;
			if(l == 0) return values.Length == 0;
			var c = str[0];
			for(int i = 0; i < values.Length; ++i)
			{
				if(values[i] == c) return true;
			}
			return false;
		}

		public static bool StartsWith(this string str, char value)
		{
			return str.Length != 0 && str[0] == value;
		}

		public static string UseAsFormat(this string str, params object[] args)
		{
			return string.Format(CultureInfo.CurrentCulture, str, args);
		}

		public static string UseAsFormat(this string str, IFormatProvider formatProvider, params object[] args)
		{
			return string.Format(formatProvider, str, args);
		}

		public static Substring GetSubstring(this string str, int start, int length)
		{
			return new Substring(str, start, length);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}
	}
}
