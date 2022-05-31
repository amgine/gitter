#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Layout;

public static class SizeSpecExtensions
{
	sealed class MinConstrainedSizeSpec : ISizeSpec
	{
		public MinConstrainedSizeSpec(ISizeSpec sizeSpec)
		{
			SizeSpec = sizeSpec;
			MinSize  = sizeSpec.AsScalable();
		}

		public MinConstrainedSizeSpec(ISizeSpec sizeSpec, IDpiBoundValue<int> minSize)
		{
			SizeSpec = sizeSpec;
			MinSize  = minSize;
		}

		private ISizeSpec SizeSpec { get; }

		private IDpiBoundValue<int> MinSize { get; }

		public int Priority => SizeSpec.Priority;

		public int GetSize(int available, Dpi dpi)
		{
			var min = MinSize.GetValue(dpi);
			if(available < min) return 0;
			return SizeSpec.GetSize(available, dpi);
		}
	}

	private sealed class SizeSpecAsScalable : IDpiBoundValue<int>
	{
		public SizeSpecAsScalable(ISizeSpec sizeSpec) => SizeSpec = sizeSpec;

		private ISizeSpec SizeSpec { get; }

		public int GetValue(Dpi dpi) => SizeSpec.GetSize(int.MaxValue, dpi);
	}

	public static IDpiBoundValue<int> AsScalable(this ISizeSpec sizeSpec)
		=> new SizeSpecAsScalable(sizeSpec);

	public static ISizeSpec CollapseIfTooSmall(this ISizeSpec sizeSpec)
	{
		Verify.Argument.IsNotNull(sizeSpec);

		return new MinConstrainedSizeSpec(sizeSpec);
	}

	public static ISizeSpec CollapseIfTooSmall(this ISizeSpec sizeSpec, IDpiBoundValue<int> minSize)
	{
		Verify.Argument.IsNotNull(sizeSpec);
		Verify.Argument.IsNotNull(minSize);

		return new MinConstrainedSizeSpec(sizeSpec, minSize);
	}
}
