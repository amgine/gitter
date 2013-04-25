namespace gitter.Git.AccessLayer
{
	public sealed class ObjectCountData
	{
		#region Data

		private readonly int _count;
		private readonly int _size;
		private readonly int _inPack;
		private readonly int _packs;
		private readonly int _sizePack;
		private readonly int _prunePackable;
		private readonly int _garbage;

		#endregion

		#region .ctor

		public ObjectCountData(int count, int size, int inPack, int packs, int sizePack, int prunePackable, int garbage)
		{
			_count = count;
			_size = size;
			_inPack = inPack;
			_packs = packs;
			_sizePack = sizePack;
			_prunePackable = prunePackable;
			_garbage = garbage;
		}

		#endregion

		#region Properties

		public int Count
		{
			get { return _count; }
		}

		public int Size
		{
			get { return _size; }
		}

		public int InPack
		{
			get { return _inPack; }
		}

		public int Packs
		{
			get { return _packs; }
		}

		public int PrunePackable
		{
			get { return _prunePackable; }
		}

		public int Garbage
		{
			get { return _garbage; }
		}

		#endregion
	}
}
