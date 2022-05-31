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

public static class SizeSpec
{
	sealed record class ConstantSize(int Value) : ISizeSpec
	{
		public static ISizeSpec Zero { get; } = new ConstantSize(0);

		public int Priority => 1;

		public int GetSize(int available, Dpi dpi) => Value;
	}

	sealed record class DpiScaledSize(int OriginalDpi, int Value) : ISizeSpec
	{
		public int Priority => 1;

		public int GetSize(int available, Dpi dpi) => (2 * Value * dpi.X + 1) / (2 * OriginalDpi);
	}

	sealed record class ScalableSize(IDpiBoundValue<int> Scalable) : ISizeSpec
	{
		public int Priority => 1;

		public int GetSize(int available, Dpi dpi)
			=> Scalable.GetValue(dpi);
	}

	sealed record class RelativeSize(int Priority, float Value) : ISizeSpec
	{
		public int GetSize(int available, Dpi dpi) => (int)(available * Value);
	}

	sealed class AllRemainingSize : ISizeSpec
	{
		public static ISizeSpec Instance { get; } = new AllRemainingSize();

		public int Priority => 0;

		public int GetSize(int available, Dpi dpi) => available;
	}

	public static ISizeSpec Absolute(int size, bool scaleToDpi = true)
		=> scaleToDpi ? new DpiScaledSize(96, size) : new ConstantSize(size);

	public static ISizeSpec Absolute(IDpiBoundValue<int> scalable)
		=> new ScalableSize(scalable);

	public static ISizeSpec Relative(float value)
		=> new RelativeSize(1, value);

	public static ISizeSpec RelativeToLeftover(float value)
		=> new RelativeSize(0, value);

	public static ISizeSpec Everything() => AllRemainingSize.Instance;

	public static ISizeSpec Nothing() => ConstantSize.Zero;
}
