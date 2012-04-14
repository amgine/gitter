namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Security.Cryptography;
	using System.Drawing;
	using System.Net;

	/// <summary>Service for requesting global avatars.</summary>
	public static class GravatarService
	{
		private static readonly MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
		private static readonly char[] Alphabet = new char[] { '0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f' };

		private const string URL = "http://www.gravatar.com/avatar/{0}?d={1}&s={2}&r={3}";

		public const int DefaultSize = 80;

		private static string MD5(string email)
		{
			var hash = md5Provider.ComputeHash(Encoding.ASCII.GetBytes(email.ToLower()));
			var arr = new char[hash.Length * 2];
			for(int i = 0, j = 0; i < hash.Length; ++i)
			{
				var h = hash[i];
				arr[j++] = Alphabet[h >> 4];
				arr[j++] = Alphabet[h & 0x0f];
			}
			return new string(arr);
		}

		private static Bitmap ExtractGravatar(WebResponse response)
		{
			using(var stream = response.GetResponseStream())
			{
				return new Bitmap(stream);
			}
		}

		public static Bitmap GetGravatar(string email)
		{
			return GetGravatar(email, DefaultGravatarType.wavatar, GravatarRating.g, 80);
		}

		public static Bitmap GetGravatar(string email, DefaultGravatarType defaultType, GravatarRating rating, int size)
		{
			var url = string.Format(URL, MD5(email), defaultType, size, rating);
			var http = HttpWebRequest.Create(url);
			using(var response = http.GetResponse())
			{
				return ExtractGravatar(response);
			}
		}

		public static IAsyncResult BeginGetGravatar(AsyncCallback callback, string email)
		{
			return BeginGetGravatar(callback, email, DefaultGravatarType.wavatar, GravatarRating.g, 80);
		}

		public static IAsyncResult BeginGetGravatar(AsyncCallback callback, string email, DefaultGravatarType defaultType, GravatarRating rating, int size)
		{
			var func = new Func<string, DefaultGravatarType, GravatarRating, int, Bitmap>(GetGravatar);
			return func.BeginInvoke(email, defaultType, rating, size, callback, func);
		}

		public static Bitmap EndGetGravatar(IAsyncResult result)
		{
			if(result == null) throw new ArgumentNullException("result");
			var func = (Func<string, DefaultGravatarType, GravatarRating, int, Bitmap>)result.AsyncState;
			return func.EndInvoke(result);
		}
	}

	public enum DefaultGravatarType
	{
		identicon,
		monsterid,
		wavatar
	}

	public enum GravatarRating
	{
		g,
		pg,
		r,
		x
	}
}
