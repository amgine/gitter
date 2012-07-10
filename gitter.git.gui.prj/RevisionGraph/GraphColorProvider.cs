namespace gitter.Git.Gui
{
	/// <summary>Object for unique color allocation.</summary>
	public sealed class GraphColorProvider : IGraphColorProvider
	{
		private int _maxColors;
		private readonly bool[] _colors;
		private int _pointer;

		/// <summary>Create <see cref="GraphColorProvider"/>.</summary>
		/// <param name="maxColors">Maximum colors.</param>
		public GraphColorProvider(int maxColors)
		{
			_maxColors = maxColors;
			_colors = new bool[maxColors];
			_pointer = 0;
		}

		/// <summary>Create <see cref="GraphColorProvider"/>.</summary>
		public GraphColorProvider()
			: this(GraphColors.TotalColors)
		{
		}

		/// <summary>Aquire a unique color.</summary>
		/// <returns>Unique color.</returns>
		public int AcquireColor()
		{
			for(int i = (_pointer + 1) % _maxColors; i != _pointer; i = (i + 1) % _maxColors)
			{
				if(i != 0 && !_colors[i])
				{
					_colors[i] = true;
					_pointer = i;
					return i;
				}
			}
			_pointer = 1;
			return 0;
		}

		/// <summary>Make <paramref name="color"/> available again.</summary>
		/// <param name="color">Color that is not needed anymore.</param>
		public void ReleaseColor(int color)
		{
			if(color != 0)
				_colors[color] = false;
		}
	}
}
