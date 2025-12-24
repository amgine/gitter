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
	private IntPtr _handle;

	private void HunspellInit(byte[] affixData, byte[] dictionaryData, string? key)
	{
		if(_handle != IntPtr.Zero)
			throw new InvalidOperationException("Dictionary is already loaded");
		MarshalHunspellDll.ReferenceNativeHunspellDll();
		nativeDllIsReferenced = true;
		_handle = MarshalHunspellDll.HunspellInit(
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

	~Hunspell() => Dispose(disposing: false);

	public static string NativeDllPath
	{
		get => MarshalHunspellDll.NativeDLLPath;
		set => MarshalHunspellDll.NativeDLLPath = value;
	}

	public bool IsDisposed { get; private set; }

	private IntPtr GetHandle()
	{
		var handle = _handle;
		if(handle == IntPtr.Zero) throw new InvalidOperationException("Dictionary is not loaded");
		return handle;
	}

	public bool Add(string word)
	{
		_ = MarshalHunspellDll.HunspellAdd(GetHandle(), word);
		return Spell(word);
	}

	public bool AddWithAffix(string word, string example)
	{
		_ = MarshalHunspellDll.HunspellAddWithAffix(GetHandle(), word, example);
		return Spell(word);
	}

	public bool Remove(string word)
	{
		_ = MarshalHunspellDll.HunspellRemove(GetHandle(), word);
		return !Spell(word);
	}

	public bool Spell(string word)
		=> MarshalHunspellDll.HunspellSpell(GetHandle(), word);

	public List<string> Analyze(string word)
		=> ToStringList(MarshalHunspellDll.HunspellAnalyze(GetHandle(), word));

	public List<string> Generate(string word, string sample)
		=> ToStringList(MarshalHunspellDll.HunspellGenerate(GetHandle(), word, sample));

	public List<string> Suggest(string word)
		=> ToStringList(MarshalHunspellDll.HunspellSuggest(GetHandle(), word));

	public List<string> Stem(string word)
		=> ToStringList(MarshalHunspellDll.HunspellStem(GetHandle(), word));

	private static List<string> ToStringList(IntPtr ptr1)
	{
		if(ptr1 == IntPtr.Zero) return [];

		int num = 0;
		var stringList = new List<string>();
		for(IntPtr ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size); ptr2 != IntPtr.Zero; ptr2 = Marshal.ReadIntPtr(ptr1, num * IntPtr.Size))
		{
			++num;
			stringList.Add(Marshal.PtrToStringUni(ptr2) ?? "");
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
		if(_handle != IntPtr.Zero)
		{
			MarshalHunspellDll.HunspellFree(_handle);
			_handle = IntPtr.Zero;
		}
		if(nativeDllIsReferenced)
		{
			MarshalHunspellDll.UnReferenceNativeHunspellDll();
			nativeDllIsReferenced = false;
		}
	}

	public void Load(string affFile, string dictFile, string? key = default)
	{
		affFile = Path.GetFullPath(affFile);
		if(!File.Exists(affFile))  throw new FileNotFoundException("AFF File not found: " + affFile);
		dictFile = Path.GetFullPath(dictFile);
		if(!File.Exists(dictFile)) throw new FileNotFoundException("DIC File not found: " + dictFile);
		var affixFileData      = File.ReadAllBytes(affFile);
		var dictionaryFileData = File.ReadAllBytes(dictFile);
		Load(affixFileData, dictionaryFileData, key);
	}

	public void Load(byte[] affixFileData, byte[] dictionaryFileData, string? key = default)
		=> HunspellInit(affixFileData, dictionaryFileData, key);
}
