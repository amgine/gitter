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

	public class TimeCache<T> : PersistentCache<T>
	{
		#region Data

		private TimeSpan _lifetime;
		private DateTime _cacheTime;

		#endregion

		public TimeCache(Func<T> onReevaluate, TimeSpan lifetime)
			: base(onReevaluate)
		{
			Verify.Argument.IsNotNull(onReevaluate, nameof(onReevaluate));

			_lifetime = lifetime;
		}

		public TimeCache(Func<T> onReevaluate, TimeSpan lifetime, T value)
			: base(onReevaluate, value)
		{
			_cacheTime = DateTime.Now;
		}

		public override bool IsCached
		{
			get { return base.IsCached && (DateTime.Now - _cacheTime < _lifetime); }
		}

		protected override void Reevaluate()
		{
			base.Reevaluate();
			_cacheTime = DateTime.Now;
		}
	}
}
