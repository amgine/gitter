namespace gitter.Framework
{
	public struct FileSize
	{
		private readonly long _size;
		private readonly string _units;
		private readonly string _shortSize;

		private static readonly string[] KnownUnits = new[]
		{
			"B", "KB", "MB", "GB", "TB", "PB"
		};

		public FileSize(long size)
		{
			_size = size;
			double s = size;
			var sizeId = 0;
			while(s > 1024)
			{
				s /= 1024;
				++sizeId;
			}
			if(sizeId >= KnownUnits.Length)
			{
				_shortSize = size.ToString();
				_units = "";
			}
			else
			{
				_shortSize = ((int)(s + .5)).ToString();
				_units = KnownUnits[sizeId];
			}
		}

		public long Size
		{
			get { return _size; }
		}

		public string ShortSize
		{
			get { return _shortSize; }
		}

		public string ShortSizeUnits
		{
			get { return _units; }
		}

		public override string ToString()
		{
			return ShortSize + " " + ShortSizeUnits;
		}
	}
}
