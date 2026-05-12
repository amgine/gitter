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

namespace gitter.GitLab.Api;

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using gitter.Framework;

sealed class GitLabAvatar : IAvatar
{
	private static readonly HttpClient _client = new();
	private static readonly Lazy<Bitmap> _failedImage = new(() => new Bitmap(1, 1));
	private static readonly ConcurrentDictionary<string, GitLabAvatar> _cache = new(StringComparer.Ordinal);

	public static IAvatar? For(string? url)
		=> string.IsNullOrEmpty(url) ? null : _cache.GetOrAdd(url!, static u => new GitLabAvatar(u));

	private readonly string _url;
	private Task<Image?>? _task;

	private GitLabAvatar(string url) { _url = url; }

	public event EventHandler? Updated;

	public Image? Image { get; private set; }

	public bool IsLoaded => Image is not null;

	public bool IsAvailable => true;

	public Task<Image?> UpdateAsync()
	{
		if(_task is not null) return _task;
		_task = UpdateCoreAsync();
		return _task;
	}

	private async Task<Image?> UpdateCoreAsync()
	{
		try
		{
			var data = await _client.GetByteArrayAsync(_url).ConfigureAwait(false);
			using var ms = new MemoryStream(data, writable: false);
			using var loaded = new Bitmap(ms);
			Image = new Bitmap(loaded);
		}
		catch
		{
			Image = _failedImage.Value;
		}
		Updated?.Invoke(this, EventArgs.Empty);
		return Image;
	}
}
