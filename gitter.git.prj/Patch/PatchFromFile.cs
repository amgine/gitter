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

	/// <summary>Patch from file.</summary>
	public sealed class PatchFromFile : IPatchSource
	{
		private readonly string _fileName;

		private sealed class PatchFile : IPatchFile
		{
			public PatchFile(string fileName) => FileName = fileName;

			public string FileName { get; }

			public void Dispose() { }
		}

		public PatchFromFile(string displayName, string fileName)
		{
			DisplayName = displayName;
			_fileName = fileName;
		}

		public PatchFromFile(string fileName)
			: this(fileName, fileName)
		{
		}

		public IPatchFile PreparePatchFile()
			=> new PatchFile(_fileName);

		public string DisplayName { get; }
	}
}
