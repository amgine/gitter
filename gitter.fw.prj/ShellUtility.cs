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
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Drawing;
	using System.Runtime.InteropServices;

	using Microsoft.Win32;

	using gitter.Native;

	public static class ShellUtility
	{
		public static Icon ExtractAssociatedFileIcon16ByExt(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;

			var info = new SHFILEINFO();
			Shell32.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref info, Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				return null;
			}
		}

		public static Icon ExtractAssociatedFileIcon16(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;

			var info = new SHFILEINFO();
			Shell32.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref info, Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				Shell32.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL, ref info, Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
				try
				{
					return Icon.FromHandle(info.hIcon);
				}
				catch
				{
					return null;
				}
			}
		}

		public static Icon ExtractAssociatedFolderIcon16(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;
			const int FILE_ATTRIBUTE_DIR = 0x10;

			var info = new SHFILEINFO();
			Shell32.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIR, ref info, Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				Shell32.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIR, ref info, Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
				try
				{
					return Icon.FromHandle(info.hIcon);
				}
				catch
				{
					return null;
				}
			}
		}

		public static Icon ExtractAssociatedFolderIcon16ByType(string fileName)
		{
			const int SHGFI_ICON = 0x100;
			const int SHGFI_SMALLICON = 0x1;
			const int SHGFI_USEFILEATTRIBUTES = 0x10;

			const int FILE_ATTRIBUTE_NORMAL = 0x80;
			const int FILE_ATTRIBUTE_DIR = 0x10;

			var info = new SHFILEINFO();
			Shell32.SHGetFileInfo(fileName, FILE_ATTRIBUTE_NORMAL | FILE_ATTRIBUTE_DIR, ref info, Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES);
			try
			{
				return Icon.FromHandle(info.hIcon);
			}
			catch
			{
				return null;
			}
		}

		public static Icon ExtractAssociatedIcon16_(string fileName)
		{
			var pos1 = fileName.LastIndexOf(Path.DirectorySeparatorChar);
			var pos2 = fileName.LastIndexOf('.');
			string ext;
			if(pos2 > pos1)
			{
				ext = fileName.Substring(pos2);
			}
			else
			{
				ext = fileName.Substring(pos1 + 1);
			}
			if(ext.Equals(".ico", StringComparison.InvariantCultureIgnoreCase))
			{
				return new Icon(fileName, 16, 16);
			}
			try
			{
				using(var key = Registry.ClassesRoot.OpenSubKey(ext))
				{
					var alias = (string)key.GetValue(null);
					key.Close();
					using(var aliasKey = Registry.ClassesRoot.OpenSubKey(alias + @"\DefaultIcon"))
					{
						var desc = (string)aliasKey.GetValue(null);
						var file = desc;
						var id = 0;
						var pos = desc.LastIndexOf(',');
						if(pos != -1)
						{
							if(int.TryParse(desc.Substring(pos + 1), out id))
							{
								file = desc.Substring(0, pos);
							}
						}
						aliasKey.Close();
						if(file == "%1") file = fileName;
						IntPtr[] icons = new IntPtr[1];
						var c = Shell32.ExtractIconEx(file, id, null, icons, 1);
						if(c == 1)
						{
							return Icon.FromHandle(icons[0]);
						}
						else
						{
							return null;
						}
					}
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
