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

namespace gitter.Git
{
	using System.IO;

	/// <summary>Patch from string.</summary>
	public sealed class PatchFromString : IPatchSource
	{
		private readonly string _patch;

		private sealed class PatchFile : IPatchFile
		{
			public PatchFile(string fileName) => FileName = fileName;

			public string FileName { get; }

			public void Dispose()
			{
				try
				{
					File.Delete(FileName);
				}
				catch
				{
				}
			}
		}

		public PatchFromString(string displayName, string patch)
		{
			DisplayName = displayName;
			_patch = patch;
		}

		public PatchFromString(string patch)
			: this(string.Empty, patch)
		{
		}

		public PatchFromString(string displayName, Diff patch)
			: this(displayName, patch.ToString())
		{
		}

		public PatchFromString(Diff patch)
			: this(string.Empty, patch.ToString())
		{
		}

		public IPatchFile PreparePatchFile()
		{
			var file = Path.GetTempFileName();
			File.WriteAllText(file, _patch);
			return new PatchFile(file);
		}

		public string DisplayName { get; }
	}
}
