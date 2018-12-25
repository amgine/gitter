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

	public class PersistentCache<T> : Cache<T>
	{
		#region Data

		private readonly Func<T> _onReevaluate;
		private T _value;
		private bool _isCached;

		#endregion

		public PersistentCache(Func<T> onReevaluate)
		{
			Verify.Argument.IsNotNull(onReevaluate, nameof(onReevaluate));

			_onReevaluate = onReevaluate;
		}

		public PersistentCache(Func<T> onReevaluate, T value)
		{
			Verify.Argument.IsNotNull(onReevaluate, nameof(onReevaluate));

			_onReevaluate = onReevaluate;
			_value = value;
			_isCached = true;
		}

		public override bool IsCached
		{
			get { return _isCached; }
		}

		public override T Value
		{
			get
			{
				if(!IsCached)
					Reevaluate();
				return _value;
			}
		}

		protected virtual void Reevaluate()
		{
			_onReevaluate();
			_isCached = true;
		}

		public override void Invalidate()
		{
			_isCached = false;
			_value = default(T);
		}
	}
}
