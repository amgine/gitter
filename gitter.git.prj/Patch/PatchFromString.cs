namespace gitter.Git
{
	using System;
	using System.IO;

	/// <summary>Patch from string.</summary>
	public sealed class PatchFromString : IPatchSource
	{
		private readonly string _displayName;
		private readonly string _patch;

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
				try
				{
					File.Delete(_fileName);
				}
				catch
				{
				}
			}
		}

		public PatchFromString(string displayName, string patch)
		{
			_displayName = displayName;
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

		public string DisplayName
		{
			get { return _displayName; }
		}
	}
}
