namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	/// <summary>Patch from file.</summary>
	public sealed class PatchFromFile : IPatchSource
	{
		private readonly string _displayName;
		private readonly string _fileName;

		private sealed class PatchFile : IPatchFile
		{
			private readonly string _fileName;

			public PatchFile(string fileName)
			{
				_fileName = fileName;
			}

			public string FileName
			{
				get { return _fileName; }
			}

			public void Dispose()
			{
			}
		}

		public PatchFromFile(string displayName, string fileName)
		{
			_displayName = displayName;
			_fileName = fileName;
		}

		public PatchFromFile(string fileName)
			: this(fileName, fileName)
		{
		}

		public IPatchFile PreparePatchFile()
		{
			return new PatchFile(_fileName);
		}

		public string DisplayName
		{
			get { return _displayName; }
		}
	}
}
