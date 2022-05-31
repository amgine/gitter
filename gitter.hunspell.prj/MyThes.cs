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
using System.Text;

namespace NHunspell;

public class MyThes
{
	private readonly object dictionaryLock = new();
	private readonly Dictionary<string, ThesMeaning[]> synonyms = new();

	public MyThes()
	{
	}

	public MyThes(byte[] datBytes) => Load(datBytes);

	public MyThes(string datFile) => Load(datFile);

	public static Encoding GetEncoding(string encoding)
	{
		encoding = encoding.Trim().ToLower();
		return encoding switch
		{
			@"utf-8"        or @"utf8"             => Encoding.GetEncoding(65001),
			@"iso8859-1"    or @"iso-8859-1"       => Encoding.GetEncoding(28591),
			@"iso8859-2"    or @"iso-8859-2"       => Encoding.GetEncoding(28592),
			@"iso8859-3"    or @"iso-8859-3"       => Encoding.GetEncoding(28593),
			@"iso8859-4"    or @"iso-8859-4"       => Encoding.GetEncoding(28594),
			@"iso8859-5"    or @"iso-8859-5"       => Encoding.GetEncoding(28595),
			@"iso8859-6"    or @"iso-8859-6"       => Encoding.GetEncoding(28596),
			@"iso8859-7"    or @"iso-8859-7"       => Encoding.GetEncoding(28597),
			@"iso8859-8"    or @"iso-8859-8"       => Encoding.GetEncoding(28598),
			@"iso8859-9"    or @"iso-8859-9"       => Encoding.GetEncoding(28599),
			@"iso8859-13"   or @"iso-8859-13"      => Encoding.GetEncoding(28603),
			@"iso8859-15"   or @"iso-8859-15"      => Encoding.GetEncoding(28605),
			@"windows-1250" or @"microsoft-cp1250" => Encoding.GetEncoding( 1250),
			@"windows-1251" or @"microsoft-cp1251" => Encoding.GetEncoding( 1251),
			@"windows-1252" or @"microsoft-cp1252" => Encoding.GetEncoding( 1252),
			@"windows-1253" or @"microsoft-cp1253" => Encoding.GetEncoding( 1253),
			@"windows-1254" or @"microsoft-cp1254" => Encoding.GetEncoding( 1254),
			@"windows-1255" or @"microsoft-cp1255" => Encoding.GetEncoding( 1255),
			@"windows-1256" or @"microsoft-cp1256" => Encoding.GetEncoding( 1256),
			@"windows-1257" or @"microsoft-cp1257" => Encoding.GetEncoding( 1257),
			@"windows-1258" or @"microsoft-cp1258" => Encoding.GetEncoding( 1258),
			@"windows-1259" or @"microsoft-cp1259" => Encoding.GetEncoding( 1259),
			@"koi8-r"       or @"koi8-u"           => Encoding.GetEncoding(20866),
			_ => throw new NotSupportedException("Encoding: " + encoding + " is not supported"),
		};
	}

	public void Load(byte[] dictionaryBytes)
	{
		if(this.synonyms.Count > 0)
			throw new InvalidOperationException("Thesaurus already loaded");
		int num1 = 0;
		int lineLength1 = this.GetLineLength(dictionaryBytes, num1);
		Encoding encoding = MyThes.GetEncoding(Encoding.ASCII.GetString(dictionaryBytes, num1, lineLength1));
		int pos = num1 + lineLength1;
		string empty = string.Empty;
		List<ThesMeaning> thesMeaningList = new List<ThesMeaning>();
		int num2;
		int lineLength2;
		for(; pos < dictionaryBytes.Length; pos = num2 + lineLength2)
		{
			num2 = pos + this.GetCrLfLength(dictionaryBytes, pos);
			lineLength2 = this.GetLineLength(dictionaryBytes, num2);
			string str = encoding.GetString(dictionaryBytes, num2, lineLength2).Trim();
			if(str != null && str.Length > 0)
			{
				string[] strArray = str.Split('|');
				if(strArray.Length > 0)
				{
					bool flag = true;
					if(string.IsNullOrEmpty(strArray[0]))
						flag = false;
					else if(strArray[0].StartsWith("-"))
						flag = false;
					else if(strArray[0].StartsWith("(") && strArray[0].EndsWith(")"))
						flag = false;
					if(flag)
					{
						lock(this.dictionaryLock)
						{
							if(empty.Length > 0)
							{
								if(!this.synonyms.ContainsKey(empty.ToLowerInvariant()))
									this.synonyms.Add(empty.ToLowerInvariant(), thesMeaningList.ToArray());
							}
						}
						thesMeaningList = new List<ThesMeaning>();
						empty = strArray[0];
					}
					else
					{
						List<string> synonyms = new List<string>();
						string description = (string)null;
						for(int index = 1; index < strArray.Length; ++index)
						{
							synonyms.Add(strArray[index]);
							if(index == 1)
								description = strArray[index];
						}
						ThesMeaning thesMeaning = new ThesMeaning(description, synonyms);
						thesMeaningList.Add(thesMeaning);
					}
				}
			}
		}
		lock(this.dictionaryLock)
		{
			if(empty.Length <= 0 || this.synonyms.ContainsKey(empty.ToLowerInvariant()))
				return;
			this.synonyms.Add(empty.ToLowerInvariant(), thesMeaningList.ToArray());
		}
	}

	public void Load(string dictionaryFile)
	{
		dictionaryFile = Path.GetFullPath(dictionaryFile);
		if(!File.Exists(dictionaryFile))
			throw new FileNotFoundException("DAT File not found: " + dictionaryFile);
		byte[] dictionaryBytes;
		using(FileStream input = File.OpenRead(dictionaryFile))
		{
			using(BinaryReader binaryReader = new BinaryReader((Stream)input))
				dictionaryBytes = binaryReader.ReadBytes((int)input.Length);
		}
		this.Load(dictionaryBytes);
	}

	public ThesResult Lookup(string word)
	{
		if(this.synonyms.Count == 0)
			throw new InvalidOperationException("Thesaurus not loaded");
		word = word.ToLowerInvariant();
		ThesMeaning[] collection;
		lock(this.dictionaryLock)
		{
			if(!this.synonyms.TryGetValue(word, out collection))
				return (ThesResult)null;
		}
		return new ThesResult(new List<ThesMeaning>((IEnumerable<ThesMeaning>)collection), false);
	}

	public ThesResult Lookup(string word, Hunspell stemming)
	{
		if(this.synonyms.Count == 0)
			throw new InvalidOperationException("Thesaurus not loaded");
		ThesResult thesResult1 = this.Lookup(word);
		if(thesResult1 != null)
			return thesResult1;
		List<string> stringList = stemming.Stem(word);
		if(stringList == null || stringList.Count == 0)
			return (ThesResult)null;
		List<ThesMeaning> meanings = new List<ThesMeaning>();
		foreach(string word1 in stringList)
		{
			ThesResult thesResult2 = this.Lookup(word1);
			if(thesResult2 != null)
			{
				foreach(ThesMeaning meaning in thesResult2.Meanings)
				{
					List<string> synonyms = new List<string>();
					foreach(string synonym in meaning.Synonyms)
					{
						foreach(string str in stemming.Generate(synonym, word))
							synonyms.Add(str);
					}
					if(synonyms.Count > 0)
						meanings.Add(new ThesMeaning(meaning.Description, synonyms));
				}
			}
		}
		return meanings.Count > 0 ? new ThesResult(meanings, true) : (ThesResult)null;
	}

	private int GetCrLfLength(byte[] buffer, int pos)
	{
		if(buffer[pos] == (byte)10)
			return buffer.Length > pos + 1 && buffer[pos] == (byte)13 ? 2 : 1;
		if(buffer[pos] != (byte)13)
			throw new ArgumentException("buffer[pos] dosen't point to CR or LF");
		return buffer.Length > pos + 1 && buffer[pos] == (byte)10 ? 2 : 1;
	}

	private int GetLineLength(byte[] buffer, int start)
	{
		for(int index = start; index < buffer.Length; ++index)
		{
			if(buffer[index] == (byte)10 || buffer[index] == (byte)13)
				return index - start;
		}
		return buffer.Length - start;
	}
}
