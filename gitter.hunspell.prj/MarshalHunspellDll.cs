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

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NHunspell;

internal static class MarshalHunspellDll
{
	private static readonly object nativeDllReferenceCountLock = new();

	internal static HunspellAddDelegate HunspellAdd;
	internal static HunspellAddWithAffixDelegate HunspellAddWithAffix;
	internal static HunspellAnalyzeDelegate HunspellAnalyze;
	internal static HunspellFreeDelegate HunspellFree;
	internal static HunspellGenerateDelegate HunspellGenerate;
	internal static HunspellInitDelegate HunspellInit;
	internal static HunspellRemoveDelegate HunspellRemove;
	internal static HunspellSpellDelegate HunspellSpell;
	internal static HunspellStemDelegate HunspellStem;
	internal static HunspellSuggestDelegate HunspellSuggest;
	internal static HyphenFreeDelegate HyphenFree;
	internal static HyphenHyphenateDelegate HyphenHyphenate;
	internal static HyphenInitDelegate HyphenInit;

	private static IntPtr dllHandle = IntPtr.Zero;
	private static string nativeDLLPath;
	private static int nativeDllReferenceCount;

	internal static string NativeDLLPath
	{
		get => nativeDLLPath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty);
		set
		{
			if(dllHandle != IntPtr.Zero) throw new InvalidOperationException("Native Library is already loaded");
			nativeDLLPath = value;
		}
	}

#if !NET5_0_OR_GREATER

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
	internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

	[DllImport("kernel32.dll")]
	internal static extern IntPtr LoadLibrary(string fileName);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool FreeLibrary(IntPtr hModule);

#endif

	internal static void ReferenceNativeHunspellDll()
	{
		lock(nativeDllReferenceCountLock)
		{
			if(nativeDllReferenceCount == 0)
			{
				if(dllHandle != IntPtr.Zero) throw new InvalidOperationException("Native DLL handle is not Zero");
				try
				{
					var fileName = RuntimeInformation.ProcessArchitecture switch
					{
						Architecture.X86 => "Hunspellx86.dll",
						Architecture.X64 => "Hunspellx64.dll",
						_ => throw new PlatformNotSupportedException(),
					};
					fileName = Path.Combine(NativeDLLPath, fileName);

#if NET5_0_OR_GREATER
					dllHandle = NativeLibrary.Load(fileName);
#else
					dllHandle = LoadLibrary(fileName);
					if(dllHandle == IntPtr.Zero)
						throw new DllNotFoundException($"Hunspell DLL not found: {fileName}");
#endif

					HunspellInit         = GetDelegate<HunspellInitDelegate        >(@"HunspellInit");
					HunspellFree         = GetDelegate<HunspellFreeDelegate        >(@"HunspellFree");
					HunspellAdd          = GetDelegate<HunspellAddDelegate         >(@"HunspellAdd");
					HunspellAddWithAffix = GetDelegate<HunspellAddWithAffixDelegate>(@"HunspellAddWithAffix");
					HunspellRemove       = GetDelegate<HunspellRemoveDelegate      >(@"HunspellRemove");
					HunspellSpell        = GetDelegate<HunspellSpellDelegate       >(@"HunspellSpell");
					HunspellSuggest      = GetDelegate<HunspellSuggestDelegate     >(@"HunspellSuggest");
					HunspellAnalyze      = GetDelegate<HunspellAnalyzeDelegate     >(@"HunspellAnalyze");
					HunspellStem         = GetDelegate<HunspellStemDelegate        >(@"HunspellStem");
					HunspellGenerate     = GetDelegate<HunspellGenerateDelegate    >(@"HunspellGenerate");
					HyphenInit           = GetDelegate<HyphenInitDelegate          >(@"HyphenInit");
					HyphenFree           = GetDelegate<HyphenFreeDelegate          >(@"HyphenFree");
					HyphenHyphenate      = GetDelegate<HyphenHyphenateDelegate     >(@"HyphenHyphenate");
				}
				catch
				{
					if(dllHandle != IntPtr.Zero)
					{
#if NET5_0_OR_GREATER
						NativeLibrary.Free(dllHandle);
#else
						FreeLibrary(dllHandle);
#endif
						dllHandle = IntPtr.Zero;
					}
					throw;
				}
			}
			++nativeDllReferenceCount;
		}
	}

	internal static void UnReferenceNativeHunspellDll()
	{
		lock(nativeDllReferenceCountLock)
		{
			if(nativeDllReferenceCount <= 0)
				throw new InvalidOperationException("native DLL reference count is <= 0");
			--nativeDllReferenceCount;
			if(nativeDllReferenceCount != 0)
				return;
			if(dllHandle == IntPtr.Zero)
				throw new InvalidOperationException("Native DLL handle is Zero");
#if NET5_0_OR_GREATER
			NativeLibrary.Free(dllHandle);
#else
			FreeLibrary(dllHandle);
#endif
			dllHandle = IntPtr.Zero;
		}
	}

	private static T GetDelegate<T>(string procName) where T : Delegate
	{
#if NET5_0_OR_GREATER
		IntPtr procAddress = NativeLibrary.GetExport(dllHandle, procName);
#else
		IntPtr procAddress = MarshalHunspellDll.GetProcAddress(MarshalHunspellDll.dllHandle, procName);
		if(procAddress == IntPtr.Zero) throw new EntryPointNotFoundException("Function: " + procName);
#endif
		return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
	}

	internal delegate bool HunspellAddDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

	internal delegate bool HunspellAddWithAffixDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word, [MarshalAs(UnmanagedType.LPWStr)] string example);

	internal delegate IntPtr HunspellAnalyzeDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

	internal delegate void HunspellFreeDelegate(IntPtr handle);

	internal delegate IntPtr HunspellGenerateDelegate(
		IntPtr handle,
		[MarshalAs(UnmanagedType.LPWStr)] string word,
		[MarshalAs(UnmanagedType.LPWStr)] string word2);

	internal delegate IntPtr HunspellInitDelegate(
		[MarshalAs(UnmanagedType.LPArray)] byte[] affixData,
		nint affixDataSize,
		[MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData,
		nint dictionaryDataSize,
		string key);

	internal delegate bool HunspellRemoveDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

	internal delegate bool HunspellSpellDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

	internal delegate IntPtr HunspellStemDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

	internal delegate IntPtr HunspellSuggestDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

	internal delegate void HyphenFreeDelegate(IntPtr handle);

	internal delegate IntPtr HyphenHyphenateDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);

	internal delegate IntPtr HyphenInitDelegate([MarshalAs(UnmanagedType.LPArray)] byte[] dictData, IntPtr dictDataSize);
}
