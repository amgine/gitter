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

namespace gitter.Framework;

using System;
using System.Drawing;

public interface IFileIconProvider
{
	Bitmap? GetFileIcon(string fileName, Dpi dpi, string? languageId = null);
	Bitmap? GetFolderIcon(string folderName, Dpi dpi, bool expanded = false);
}

public static class FileIconProvider
{
	private static IFileIconProvider _current = new LegacyFileIconProvider();

	public static event EventHandler? Changed;

	public static IFileIconProvider Current
	{
		get => _current;
		set
		{
			var v = value ?? throw new ArgumentNullException(nameof(value));
			if(ReferenceEquals(_current, v)) return;
			_current = v;
			Changed?.Invoke(null, EventArgs.Empty);
		}
	}
}

internal sealed class LegacyFileIconProvider : IFileIconProvider
{
	public Bitmap? GetFileIcon(string fileName, Dpi dpi, string? languageId = null)
		=> GraphicsUtility.QueryIcon(fileName, dpi);

	public Bitmap? GetFolderIcon(string folderName, Dpi dpi, bool expanded = false)
		=> null;
}
