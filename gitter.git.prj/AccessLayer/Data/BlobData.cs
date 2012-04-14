namespace gitter.Git
{
	using System;

	public sealed class BlobData : TreeContentData
	{
		private readonly long _size;

		public BlobData(string hash, int mode, string name, long size)
			: base(hash, mode, name)
		{
			_size = size;
		}

		public override TreeContentType Type
		{
			get { return TreeContentType.Blob; }
		}

		public long Size
		{
			get { return _size; }
		}
	}
}
