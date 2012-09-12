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

			int IComparer<T>.Compare(T x, T y)
			{
				return _comparison(x, y);
			}
		}

		public static IComparer<T> AsComparer<T>(this Comparison<T> comparison)
		{
			Verify.Argument.IsNotNull(comparison, "comparison");

			return new Comparer<T>(comparison);
		}
	}
}
