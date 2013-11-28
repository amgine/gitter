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

namespace gitter.Native
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;

	[SuppressUnmanagedCodeSecurity]
	internal static class Kernel32
	{
		private const string DllName = "kernel32.dll";

		[DllImport(DllName)]
		public static extern void MoveMemory(IntPtr dest, IntPtr src, UIntPtr len);

		[DllImport(DllName)]
		public static extern void CopyMemory(IntPtr dest, IntPtr src, UIntPtr len);

		/// <summary>
		/// Multiplies two 32-bit values and then divides the 64-bit result by a third 32-bit value.
		/// The final result is rounded to the nearest integer.
		/// </summary>
		/// <param name="nNumber">The multiplicand.</param>
		/// <param name="nNumerator">The multiplier.</param>
		/// <param name="nDenominator">The number by which the result of the multiplication operation is to be divided.</param>
		/// <returns>
		/// If the function succeeds, the return value is the result of the multiplication and division, rounded to the nearest integer.
		/// If the result is a positive half integer (ends in .5), it is rounded up. If the result is a negative half integer, it is rounded down.
		/// If either an overflow occurred or nDenominator was 0, the return value is -1.
		/// </returns>
		[DllImport(DllName)]
		public static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);

		[DllImport(DllName)]
		public static extern int GetCurrentThreadId();

		[DllImport(DllName)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);
	}
}
