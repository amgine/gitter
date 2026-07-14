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

// Polyfills used by the interactive-rebase code path. Compiled only on the
// .NET Framework target (net48) where these APIs are missing.

#if NETFRAMEWORK

#pragma warning disable CS1591

namespace System
{
	internal readonly struct Index : IEquatable<Index>
	{
		private readonly int _value;
		public Index(int value, bool fromEnd = false)
		{
			if(value < 0) throw new ArgumentOutOfRangeException(nameof(value));
			_value = fromEnd ? ~value : value;
		}
		public static Index Start    => new(0);
		public static Index End      => new(~0);
		public static Index FromStart(int value) => new(value);
		public static Index FromEnd(int value)   => new(value, fromEnd: true);
		public int  Value     => _value < 0 ? ~_value : _value;
		public bool IsFromEnd => _value < 0;
		public int GetOffset(int length) => IsFromEnd ? length - (~_value) : _value;
		public static implicit operator Index(int value) => new(value);
		public bool Equals(Index other) => _value == other._value;
		public override bool Equals(object? obj) => obj is Index i && Equals(i);
		public override int GetHashCode() => _value;
	}

	internal readonly struct Range : IEquatable<Range>
	{
		public Index Start { get; }
		public Index End   { get; }
		public Range(Index start, Index end) { Start = start; End = end; }
		public static Range All => new(Index.Start, Index.End);
		public static Range StartAt(Index start) => new(start, Index.End);
		public static Range EndAt(Index end)     => new(Index.Start, end);
		public (int Offset, int Length) GetOffsetAndLength(int length)
		{
			var s = Start.GetOffset(length);
			var e = End.GetOffset(length);
			if((uint)e > (uint)length || (uint)s > (uint)e) throw new ArgumentOutOfRangeException(nameof(length));
			return (s, e - s);
		}
		public bool Equals(Range other) => Start.Equals(other.Start) && End.Equals(other.End);
		public override bool Equals(object? obj) => obj is Range r && Equals(r);
		public override int GetHashCode() => (Start.GetHashCode() << 16) ^ End.GetHashCode();
	}
}

namespace System.Runtime.CompilerServices
{
	internal static class RuntimeHelpersPolyfill
	{
		public static string SubstringByRange(this string s, System.Range range)
		{
			var (offset, length) = range.GetOffsetAndLength(s.Length);
			return s.Substring(offset, length);
		}
	}
}

namespace System.Diagnostics
{
	using System.Threading;
	using System.Threading.Tasks;

	internal static class ProcessPolyfillExtensions
	{
		public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
		{
			if(process is null) throw new ArgumentNullException(nameof(process));
			if(process.HasExited) return Task.CompletedTask;

			var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
			process.EnableRaisingEvents = true;
			void Handler(object? sender, EventArgs e) => tcs.TrySetResult(null);
			process.Exited += Handler;
			if(process.HasExited) tcs.TrySetResult(null);

			if(cancellationToken.CanBeCanceled)
			{
				cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
			}
			return tcs.Task;
		}
	}
}

#pragma warning restore CS1591

#endif
