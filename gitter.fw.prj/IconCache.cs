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

namespace gitter.Framework
{
	using System;
	using System.Drawing;
	using System.IO;
	using System.Collections.Generic;

	internal static class IconCache
	{
		private static readonly Dictionary<string, string> _associations = new(comparer: StringComparer.OrdinalIgnoreCase)
		{
			[@".exe"] = @"file.application",

			[@".com"] = @"file.terminal",
			[@".cmd"] = @"file.terminal",
			[@".bat"] = @"file.terminal",
			[@".ps1"] = @"file.terminal",

			[@".dll"] = @"file.binary",

			[@".xml"]  = @"file.code",
			[@".cs"]   = @"file.code",
			[@".py"]   = @"file.code",
			[@".vb"]   = @"file.code",
			[@".c"]    = @"file.code",
			[@".cpp"]  = @"file.code",
			[@".h"]    = @"file.code",
			[@".hpp"]  = @"file.code",
			[@".js"]   = @"file.code",
			[@".aspx"] = @"file.code",
			[@".php"]  = @"file.php",
			[@".xaml"] = @"file.xaml",

			[@".xlsx"] = @"file.excel",
			[@".xsl"]  = @"file.excel",
			[@".csv"]  = @"file.excel.csv",

			[@".avi"] = @"file.film",
			[@".mp4"] = @"file.film",
			[@".mov"] = @"file.film",
			[@".mkv"] = @"file.film",

			[@".flv"] = @"file.flash",

			[@".htm"]  = @"file.globe",
			[@".html"] = @"file.globe",

			[@".bmp"]  = @"file.image",
			[@".png"]  = @"file.image",
			[@".gif"]  = @"file.image",
			[@".tga"]  = @"file.image",
			[@".jpg"]  = @"file.image",
			[@".jpeg"] = @"file.image",

			[@".wav"] = @"file.music",
			[@".wma"] = @"file.music",
			[@".mp3"] = @"file.music",
			[@".ogg"] = @"file.music",
			[@".ape"] = @"file.music",

			[@".pdf"] = @"file.pdf",

			[@".psd"] = @"file.photoshop",

			[@".ppt"]  = @"file.powerpoint",
			[@".pptx"] = @"file.powerpoint",

			[@".txt"] = @"file.text",

			[@".sln"]    = @"file.vs",
			[@".csproj"] = @"file.vs",

			[@".rtf"]  = @"file.word",
			[@".doc"]  = @"file.word",
			[@".docx"] = @"file.word",
		};

		private const string _defaultIconName = @"file.default";

		private static string GetCacheKey(string fileName)
		{
			try
			{
				return Path.GetExtension(fileName);
			}
			catch
			{
				return string.Empty;
			}
		}

		public static Bitmap GetIcon(string fileName, int size)
		{
			if(!_associations.TryGetValue(GetCacheKey(fileName), out var name))
			{
				name = _defaultIconName;
			}
			return CachedResources.ScaledBitmaps[name, size];
		}
	}
}
