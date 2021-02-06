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

namespace gitter.Framework.Extensions
{
	using System;
	using System.Collections.Generic;

	public static class ComparisonExtensions
	{
		private sealed class Comparer<T> : IComparer<T>
		{
			private readonly Comparison<T> _comparison;

			public Comparer(Comparison<T> comparison)
			{
				_comparison = comparison;
			}

			int IComparer<T>.Compare(T x, T y) => _comparison(x, y);
		}

		public static IComparer<T> AsComparer<T>(this Comparison<T> comparison)
		{
			Verify.Argument.IsNotNull(comparison, nameof(comparison));

			return new Comparer<T>(comparison);
		}
	}
}
