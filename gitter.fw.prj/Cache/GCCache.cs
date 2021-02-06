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

	public class GCCache<T> : Cache<T>
		where T : class
	{
		private readonly Func<T> _onReevaluate;
		private WeakReference _weakRef;

		public GCCache(Func<T> onReevaluate)
		{
			Verify.Argument.IsNotNull(onReevaluate, nameof(onReevaluate));

			_onReevaluate = onReevaluate;
		}

		public GCCache(Func<T> onReevaluate, T value)
		{
			Verify.Argument.IsNotNull(onReevaluate, nameof(onReevaluate));

			_onReevaluate = onReevaluate;
			if(value != null) _weakRef = new WeakReference(value);
		}

		public override bool IsCached => _weakRef != null && _weakRef.IsAlive;

		public override T Value
		{
			get
			{
				if(_weakRef == null)
				{
					return Reevaluate();
				}
				else
				{
					var value = (T)_weakRef.Target;
					if(value == null)
					{
						return Reevaluate();
					}
					else
					{
						return value;
					}
				}
			}
		}

		public override void Invalidate()
		{
			_weakRef = null;
		}

		private T Reevaluate()
		{
			var value = _onReevaluate();
			_weakRef = new WeakReference(value);
			return value;
		}
	}
}
