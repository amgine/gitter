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

public class Hyphen : IDisposable
{
	private bool _nativeDllIsReferenced;
	private IntPtr _handle;

	public Hyphen()
	{
	}

	~Hyphen() => Dispose(disposing: false);

	public Hyphen(string dictFile) => Load(dictFile);

	public Hyphen(byte[] dictFileData) => Load(dictFileData);

	public static string NativeDllPath
	{
		get => MarshalHunspellDll.NativeDLLPath;
		set => MarshalHunspellDll.NativeDLLPath = value;
	}

	public bool IsDisposed { get; private set; }

	public void Dispose()
	{
		if(IsDisposed) return;

		GC.SuppressFinalize(this);
		Dispose(disposing: true);
		IsDisposed = true;
	}

	private void Dispose(bool disposing)
	{
		if(_handle != IntPtr.Zero)
		{
			MarshalHunspellDll.HyphenFree(_handle);
			_handle = IntPtr.Zero;
		}
		if(_nativeDllIsReferenced)
		{
			MarshalHunspellDll.UnReferenceNativeHunspellDll();
			_nativeDllIsReferenced = false;
		}
	}

	public HyphenResult? Hyphenate(string word)
	{
		if(this._handle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		if(word is not { Length: not 0 })
			return default;
		IntPtr ptr1 = MarshalHunspellDll.HyphenHyphenate(_handle, word);
		IntPtr ptr2 = Marshal.ReadIntPtr(ptr1);
		int size = IntPtr.Size;
		IntPtr ptr3 = Marshal.ReadIntPtr(ptr1, size);
		int ofs1 = size + IntPtr.Size;
		IntPtr ptr4 = Marshal.ReadIntPtr(ptr1, ofs1);
		int ofs2 = ofs1 + IntPtr.Size;
		IntPtr ptr5 = Marshal.ReadIntPtr(ptr1, ofs2);
		int ofs3 = ofs2 + IntPtr.Size;
		IntPtr ptr6 = Marshal.ReadIntPtr(ptr1, ofs3);
		int num = ofs3 + IntPtr.Size;
		var hyphenationPoints = new byte[Math.Max(word.Length - 1, 1)];
		var hyphenationRep    = new string[Math.Max(word.Length - 1, 1)];
		var hyphenationPos    = new int[Math.Max(word.Length - 1, 1)];
		var hyphenationCut    = new int[Math.Max(word.Length - 1, 1)];
		for(int ofs4 = 0; ofs4 < word.Length - 1; ++ofs4)
		{
			hyphenationPoints[ofs4] = Marshal.ReadByte(ptr3, ofs4);
			if(ptr4 != IntPtr.Zero)
			{
				IntPtr ptr7 = Marshal.ReadIntPtr(ptr4, ofs4 * IntPtr.Size);
				if(ptr7 != IntPtr.Zero)
					hyphenationRep[ofs4] = Marshal.PtrToStringUni(ptr7) ?? "";
				hyphenationPos[ofs4] = Marshal.ReadInt32(ptr5, ofs4 * 4);
				hyphenationCut[ofs4] = Marshal.ReadInt32(ptr6, ofs4 * 4);
			}
		}
		return new HyphenResult(Marshal.PtrToStringUni(ptr2) ?? "", hyphenationPoints, hyphenationRep, hyphenationPos, hyphenationCut);
	}

	public void Load(string dictFile)
	{
		dictFile = Path.GetFullPath(dictFile);
		Load(File.ReadAllBytes(dictFile));
	}

	public void Load(byte[] dictFileData) => Init(dictFileData);

	private void Init(byte[] dictionaryData)
	{
		if(this._handle != IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is already loaded");
		MarshalHunspellDll.ReferenceNativeHunspellDll();
		_nativeDllIsReferenced = true;
		_handle = MarshalHunspellDll.HyphenInit(dictionaryData, new IntPtr(dictionaryData.Length));
	}
}
