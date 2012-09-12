namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>Represents patch.</summary>
	public sealed class Diff : IEnumerable<DiffFile>, ICloneable
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
			Verify.Argument.IsNotNull(files, "files");
			Verify.Argument.HasNoNullItems(files, "files");

			_type = type;
			_files = files;
		}

		#endregion

		public void Add(DiffFile diffFile)
		{
			Verify.Argument.IsNotNull(diffFile, "diffFile");

			_files.Add(diffFile);
		}

		public void Insert(int index, DiffFile diffFile)
		{
			Verify.Argument.IsNotNull(diffFile, "diffFile");

			_files.Insert(index, diffFile);
		}

		public bool Remove(DiffFile diffFile)
		{
			Verify.Argument.IsNotNull(diffFile, "diffFile");

			return _files.Remove(diffFile);
		}

		public void RemoveAt(int index)
		{
			_files.RemoveAt(index);
		}

		public DiffType Type
		{
			get { return _type; }
		}

		/// <summary>Diff is empty.</summary>
		public bool IsEmpty
		{
			get { return _files.Count == 0; }
		}

		public DiffFile this[int index]
		{
			get { return _files[index]; }
		}

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

		public int FilesCount
		{
			get { return _files.Count; }
		}

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

		object ICloneable.Clone()
		{
			return Clone();
		}

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
