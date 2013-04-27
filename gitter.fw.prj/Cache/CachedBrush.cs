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
