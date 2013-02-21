namespace gitter.Framework
{
	using System;
	using System.Drawing;

	public sealed class CachedBrush : Cache<Brush>, IDisposable
	{
		#region Data

		private Func<Color> _brushColorProvider;
		private Color _cachedColor;
		private Brush _cachedBrush;

		#endregion

		#region .ctor

		public CachedBrush(Func<Color> brushColorProvider)
		{
			Verify.Argument.IsNotNull(brushColorProvider, "brushColorProvider");

			_brushColorProvider = brushColorProvider;
		}

		#endregion

		public override bool IsCached
		{
			get { return _cachedBrush != null; }
		}

		public override void Invalidate()
		{
			if(_cachedBrush != null)
			{
				_cachedBrush.Dispose();
				_cachedBrush = null;
			}
		}

		public override Brush Value
		{
			get
			{
				if(_cachedBrush != null)
				{
					var color = _brushColorProvider();
					if(_cachedColor != color)
					{
						_cachedBrush.Dispose();
						_cachedBrush = new SolidBrush(color);
						_cachedColor = color;
					}
				}
				else
				{
					_cachedBrush = new SolidBrush(_brushColorProvider());
				}
				return _cachedBrush;
			}
		}

		#region IDisposable Members

		public bool IsDisposed
		{
			get { return _brushColorProvider == null; }
		}

		public void Dispose()
		{
			if(!IsDisposed)
			{
				if(_cachedBrush != null)
				{
					_cachedBrush.Dispose();
					_cachedBrush = null;
				}
				_brushColorProvider = null;
			}
		}

		#endregion
	}
}
