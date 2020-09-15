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
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>Represents patch.</summary>
	public sealed class Diff : IReadOnlyList<DiffFile>, ICloneable
	{
		#region Data

		private readonly DiffType _type;
		private readonly IList<DiffFile> _files;

		#endregion

		#region .ctor

		/// <summary>Create empty <see cref="Diff"/>.</summary>
		/// <param name="type">Diff type.</param>
		public Diff(DiffType type)
		{
			_type = type;
			_files = new List<DiffFile>();
		}

		/// <summary>Create <see cref="Diff"/>.</summary>
		/// <param name="type">Diff type.</param>
		/// <param name="files">List of file diffs.</param>
		public Diff(DiffType type, IList<DiffFile> files)
		{
			Verify.Argument.IsNotNull(files, nameof(files));
			Verify.Argument.HasNoNullItems(files, nameof(files));

			_type = type;
			_files = files;
		}

		#endregion

		public void Add(DiffFile diffFile)
		{
			Verify.Argument.IsNotNull(diffFile, nameof(diffFile));

			_files.Add(diffFile);
		}

		public void Insert(int index, DiffFile diffFile)
		{
			Verify.Argument.IsNotNull(diffFile, nameof(diffFile));

			_files.Insert(index, diffFile);
		}

		public bool Remove(DiffFile diffFile)
		{
			Verify.Argument.IsNotNull(diffFile, nameof(diffFile));

			return _files.Remove(diffFile);
		}

		public void RemoveAt(int index)
		{
			_files.RemoveAt(index);
		}

		public DiffType Type => _type;

		/// <summary>Diff is empty.</summary>
		public bool IsEmpty => _files.Count == 0;

		public DiffFile this[int index] => _files[index];

		public DiffFile this[string name]
		{
			get
			{
				foreach(var file in _files)
				{
					string fileName;
					if(file.Status == FileStatus.Removed)
					{
						fileName = file.SourceFile;
					}
					else
					{
						fileName = file.TargetFile;
					}
					if(fileName == name) return file;
				}
				return null;
			}
		}

		int IReadOnlyCollection<DiffFile>.Count => _files.Count;

		public int FilesCount => _files.Count;

		#region IEnumerable<DiffFile> Members

		public IEnumerator<DiffFile> GetEnumerator()
		{
			return _files.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _files.GetEnumerator();
		}

		#endregion


		#region ICloneable

		public Diff Clone()
		{
			var files = new List<DiffFile>(_files.Count);
			for(int i = 0; i < _files.Count; ++i)
			{
				files[i] = _files[i].Clone();
			}
			return new Diff(_type, files);
		}

		object ICloneable.Clone() => Clone();

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach(var file in _files)
			{
				file.ToString(sb);
			}
			return sb.ToString();
		}
	}
}
