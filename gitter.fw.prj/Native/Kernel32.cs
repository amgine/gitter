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
	}
}
