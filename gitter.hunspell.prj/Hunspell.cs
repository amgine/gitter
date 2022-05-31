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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NHunspell;

public class Hunspell : IDisposable
{
	private bool nativeDllIsReferenced;
	private IntPtr unmanagedHandle;

	private void HunspellInit(byte[] affixData, byte[] dictionaryData, string key)
	{
		if(unmanagedHandle != IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is already loaded");
		MarshalHunspellDll.ReferenceNativeHunspellDll();
		nativeDllIsReferenced = true;
		unmanagedHandle = MarshalHunspellDll.HunspellInit(
			affixData, affixData.Length,
			dictionaryData, dictionaryData.Length, key);
	}

	public Hunspell()
	{
	}

	public Hunspell(string affFile, string dictFile)
		=> Load(affFile, dictFile);

	public Hunspell(string affFile, string dictFile, string key)
		=> Load(affFile, dictFile, key);

	public Hunspell(byte[] affixFileData, byte[] dictionaryFileData, string key)
		=> Load(affixFileData, dictionaryFileData, key);

	public Hunspell(byte[] affixFileData, byte[] dictionaryFileData)
		=> Load(affixFileData, dictionaryFileData);

	public static string NativeDllPath
	{
		get => MarshalHunspellDll.NativeDLLPath;
		set => MarshalHunspellDll.NativeDLLPath = value;
	}

	public bool IsDisposed { get; private set; }

	public bool Add(string word)
	{
		if(this.unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		int num = MarshalHunspellDll.HunspellAdd(this.unmanagedHandle, word) ? 1 : 0;
		return this.Spell(word);
	}

	public bool AddWithAffix(string word, string example)
	{
		if(this.unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		int num = MarshalHunspellDll.HunspellAddWithAffix(this.unmanagedHandle, word, example) ? 1 : 0;
		return this.Spell(word);
	}

	public List<string> Analyze(string word)
	{
		if(this.unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		List<string> stringList = new List<string>();
		IntPtr ptr1 = MarshalHunspellDll.HunspellAnalyze(this.unmanagedHandle, word);
		int num = 0;
		for(IntPtr ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size); ptr2 != IntPtr.Zero; ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size))
		{
			++num;
			stringList.Add(Marshal.PtrToStringUni(ptr2));
		}
		return stringList;
	}

	public void Dispose()
	{
		if(IsDisposed) return;
		IsDisposed = true;
		GC.SuppressFinalize(this);
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if(unmanagedHandle != IntPtr.Zero)
		{
			MarshalHunspellDll.HunspellFree(unmanagedHandle);
			unmanagedHandle = IntPtr.Zero;
		}
		if(nativeDllIsReferenced)
		{
			MarshalHunspellDll.UnReferenceNativeHunspellDll();
			nativeDllIsReferenced = false;
		}
	}

	public List<string> Generate(string word, string sample)
	{
		if(this.unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		var stringList = new List<string>();
		var ptr1 = MarshalHunspellDll.HunspellGenerate(this.unmanagedHandle, word, sample);
		int num = 0;
		for(var ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size); ptr2 != IntPtr.Zero; ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size))
		{
			++num;
			stringList.Add(Marshal.PtrToStringUni(ptr2));
		}
		return stringList;
	}

	public void Load(string affFile, string dictFile, string key = default)
	{
		affFile = Path.GetFullPath(affFile);
		if(!File.Exists(affFile))  throw new FileNotFoundException("AFF File not found: " + affFile);
		dictFile = Path.GetFullPath(dictFile);
		if(!File.Exists(dictFile)) throw new FileNotFoundException("DIC File not found: " + dictFile);
		var affixFileData      = File.ReadAllBytes(affFile);
		var dictionaryFileData = File.ReadAllBytes(dictFile);
		Load(affixFileData, dictionaryFileData, key);
	}

	public void Load(byte[] affixFileData, byte[] dictionaryFileData, string key = default)
		=> HunspellInit(affixFileData, dictionaryFileData, key);

	public bool Remove(string word)
	{
		if(unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		int num = MarshalHunspellDll.HunspellRemove(unmanagedHandle, word) ? 1 : 0;
		return !Spell(word);
	}

	public bool Spell(string word)
	{
		if(unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		return MarshalHunspellDll.HunspellSpell(this.unmanagedHandle, word);
	}

	public List<string> Stem(string word)
	{
		if(this.unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		var stringList = new List<string>();
		var ptr1 = MarshalHunspellDll.HunspellStem(this.unmanagedHandle, word);
		int num = 0;
		for(IntPtr ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size); ptr2 != IntPtr.Zero; ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size))
		{
			++num;
			stringList.Add(Marshal.PtrToStringUni(ptr2));
		}
		return stringList;
	}

	public List<string> Suggest(string word)
	{
		if(this.unmanagedHandle == IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is not loaded");
		var stringList = new List<string>();
		var ptr1 = MarshalHunspellDll.HunspellSuggest(this.unmanagedHandle, word);
		int num = 0;
		for(IntPtr ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size); ptr2 != IntPtr.Zero; ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size))
		{
			++num;
			stringList.Add(Marshal.PtrToStringUni(ptr2));
		}
		return stringList;
	}
}
