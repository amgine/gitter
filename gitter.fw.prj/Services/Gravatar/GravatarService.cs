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
using System.Threading;
#if NETCOREAPP
using System.Runtime.Intrinsics.X86;
#endif

#if NETCOREAPP
using gitter.Framework.Intrinsics;
#endif

/// <summary>Service for requesting global avatars.</summary>
public static class GravatarService
{
	private static readonly char[] Alphabet = ['0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'];

	private const string URL = "https://www.gravatar.com/avatar/{0}?d={1}&s={2}&r={3}";

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
#if NETCOREAPP
		if(Avx2.IsSupported)
		{
			fixed(byte* src = bytes)
			{
				Avx2HashHelper.ToHexStringFrom16Bytes(src, chars);
			}
			return new(chars, 0, 32);
		}
#endif
		for(int i = 0; i < bytes.Length; ++i)
		{
			int h = bytes[i];
			chars[(i << 1)    ] = Alphabet[h >> 4];
			chars[(i << 1) | 1] = Alphabet[h & 0x0f];
		}
		return new string(chars, 0, len);
	}

	private static string FormatUrl(string email,
		DefaultGravatarType defaultType = DefaultGravatarType.wavatar,
		GravatarRating      rating      = GravatarRating.g,
		int                 size        = DefaultSize)
	{
		Verify.Argument.IsInRange(1, size, 2048);

		return string.Format(URL, MD5(email), defaultType, size, rating);
	}

	private static Bitmap AsBitmap(byte[] data)
	{
		var ms = new MemoryStream(data, writable: false);
		return new Bitmap(ms);
	}

	public static async Task<Bitmap> GetGravatarAsync(string email,
		DefaultGravatarType defaultType       = DefaultGravatarType.wavatar,
		GravatarRating      rating            = GravatarRating.g,
		int                 size              = DefaultSize,
		CancellationToken   cancellationToken = default)
	{
		var url = FormatUrl(email, defaultType, rating, size);
#if NET6_0_OR_GREATER
		var data = await _client
			.GetByteArrayAsync(url, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
#else
		using var response = await _client
			.GetAsync(url, cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		response.EnsureSuccessStatusCode();
		cancellationToken.ThrowIfCancellationRequested();
		var data = await response.Content
#if NETCOREAPP
			.ReadAsByteArrayAsync(cancellationToken)
#else
			.ReadAsByteArrayAsync()
#endif
			.ConfigureAwait(continueOnCapturedContext: false);
		cancellationToken.ThrowIfCancellationRequested();
#endif
		return AsBitmap(data);
	}
}
