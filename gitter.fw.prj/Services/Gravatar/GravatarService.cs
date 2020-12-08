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

namespace gitter.Framework.Services
{
	using System;
	using System.Text;
	using System.Security.Cryptography;
	using System.Drawing;
	using System.Net;

	/// <summary>Service for requesting global avatars.</summary>
	public static class GravatarService
	{
		private static readonly MD5CryptoServiceProvider md5Provider = new();
		private static readonly char[] Alphabet = new char[] { '0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f' };

		private const string URL = "http://www.gravatar.com/avatar/{0}?d={1}&s={2}&r={3}";

		public const int DefaultSize = 80;

		private static string MD5(string email)
		{
			Assert.IsNotNull(email);

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
			Assert.IsNotNull(response);

			using var stream = response.GetResponseStream();
			return new Bitmap(stream);
		}

		public static Bitmap GetGravatar(string email)
		{
			Verify.Argument.IsNotNull(email, nameof(email));

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
			Verify.Argument.IsNotNull(result, nameof(result));

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
