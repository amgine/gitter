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

namespace gitter.Framework.Services;

using System;
using System.Text;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

/// <summary>Service for requesting global avatars.</summary>
public static class GravatarService
{
	private static readonly char[] Alphabet = { '0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f' };

	private const string URL = "http://www.gravatar.com/avatar/{0}?d={1}&s={2}&r={3}";

	public const int DefaultSize = 80;

	private static readonly HttpClient _client = new();

	private static unsafe string MD5(string email)
	{
		Assert.IsNotNull(email);

		var src = Encoding.ASCII.GetBytes(email.ToLower());

#if NET5_0_OR_GREATER
		const int MD5_SIZE = 16;
		Span<byte> hash = stackalloc byte[MD5_SIZE];
		if(!System.Security.Cryptography.MD5.TryHashData(src, hash, out var bytesWritten) || bytesWritten != MD5_SIZE)
		{
			return ToHexString(System.Security.Cryptography.MD5.HashData(src));
		}
#else
		using var md5 = System.Security.Cryptography.MD5.Create();
		var hash = md5.ComputeHash(src);
#endif
		return ToHexString(hash);
	}

	private static unsafe string ToHexString(
#if NET5_0_OR_GREATER
		ReadOnlySpan<byte> bytes
#else
		byte[] bytes
#endif
		)
	{
		var len   = bytes.Length * 2;
		var chars = stackalloc char[len];
		for(int i = 0; i < bytes.Length; ++i)
		{
			int h = bytes[i];
			chars[(i << 1)    ] = Alphabet[h >> 4];
			chars[(i << 1) | 1] = Alphabet[h & 0x0f];
		}
		return new string(chars, 0, len);
	}

	public static async Task<Bitmap> GetGravatarAsync(string email,
		DefaultGravatarType defaultType = DefaultGravatarType.wavatar,
		GravatarRating      rating      = GravatarRating.g,
		int                 size        = DefaultSize)
	{
		Verify.Argument.IsInRange(1, size, 2048, nameof(size));

		var url = string.Format(URL, MD5(email), defaultType, size, rating);
		using var response = await _client
			.GetAsync(url)
			.ConfigureAwait(continueOnCapturedContext: false);
		response.EnsureSuccessStatusCode();
		var data = await response.Content
			.ReadAsByteArrayAsync()
			.ConfigureAwait(continueOnCapturedContext: false);
		var ms = new MemoryStream(data, writable: false);
		return new Bitmap(ms);
	}
}
