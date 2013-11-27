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

	using Resources = gitter.Framework.Properties.Resources;

	internal static class IconCache
	{
		private static readonly Dictionary<string, Bitmap> _cache;
		private static readonly Bitmap _defaultIcon;

		static IconCache()
		{
			_cache = new Dictionary<string, Bitmap>();

			Bitmap icon;

			icon = Resources.IcoDocument;
			_defaultIcon = icon;
			icon = Resources.IcoApplication;
			_cache.Add(".exe", icon);
			icon = Resources.IcoApplicationTerminal;
			_cache.Add(".com", icon);
			_cache.Add(".cmd", icon);
			_cache.Add(".bat", icon);
			icon = Resources.IcoDocumentAccess;
			//_cache.Add("", icon);
			icon = Resources.IcoDocumentBinary;
			_cache.Add(".dll", icon);
			icon = Resources.IcoDocumentCode;
			_cache.Add(".xml", icon);
			_cache.Add(".cs", icon);
			_cache.Add(".vb", icon);
			_cache.Add(".c", icon);
			_cache.Add(".cpp", icon);
			_cache.Add(".h", icon);
			_cache.Add(".hpp", icon);
			icon = Resources.IcoDocumentExcel;
			_cache.Add(".xsl", icon);
			_cache.Add(".xslx", icon);
			icon = Resources.IcoDocumentExcelCsv;
			_cache.Add(".csv", icon);
			icon = Resources.IcoDocumentFilm;
			_cache.Add(".avi", icon);
			_cache.Add(".mp4", icon);
			_cache.Add(".mov", icon);
			_cache.Add(".mkv", icon);
			icon = Resources.IcoDocumentFlashMovie;
			_cache.Add(".swf", icon);
			icon = Resources.IcoDocumentGlobe;
			_cache.Add(".htm", icon);
			_cache.Add(".html", icon);
			icon = Resources.IcoDocumentImage;
			_cache.Add(".bmp", icon);
			_cache.Add(".png", icon);
			_cache.Add(".gif", icon);
			_cache.Add(".tga", icon);
			_cache.Add(".jpg", icon);
			icon = Resources.IcoDocumentMusic;
			_cache.Add(".wav", icon);
			_cache.Add(".wma", icon);
			_cache.Add(".mp3", icon);
			_cache.Add(".ogg", icon);
			icon = Resources.IcoDocumentOffice;
			//_cache.Add("", icon);
			icon = Resources.IcoDocumentOutlook;
			//_cache.Add("", icon);
			icon = Resources.IcoDocumentPdf;
			_cache.Add(".pdf", icon);
			icon = Resources.IcoDocumentPhotoshop;
			_cache.Add(".psd", icon);
			icon = Resources.IcoDocumentPhp;
			_cache.Add(".php", icon);
			icon = Resources.IcoDocumentPowerpoint;
			_cache.Add(".ppt", icon);
			_cache.Add(".pptx", icon);
			icon = Resources.IcoDocumentText;
			_cache.Add(".txt", icon);
			icon = Resources.IcoDocumentVisualStudio;
			_cache.Add(".sln", icon);
			_cache.Add(".csproj", icon);
			icon = Resources.IcoDocumentWord;
			_cache.Add(".rtf", icon);
			_cache.Add(".doc", icon);
			_cache.Add(".docx", icon);
			icon = Resources.IcoDocumentXaml;
			_cache.Add(".xaml", icon);
		}

		private static string GetCacheKey(string fileName)
		{
			return Path.GetExtension(fileName).ToLower();
		}

		public static Bitmap GetIcon(string fileName)
		{
			Bitmap res;
			try
			{
				if(!_cache.TryGetValue(GetCacheKey(fileName), out res))
				{
					res = _defaultIcon;
				}
			}
			catch(Exception exc)
			{
				if(exc.IsCritical())
				{
					throw;
				}
				res = _defaultIcon;
			}
			return res;
		}
	}
}
