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

namespace NHunspell;

public class LanguageConfig
{
	private string? hunspellAffFile;
	private string? hunspellDictFile;
	private string? hyphenDictFile;
	private string? languageCode;
	private string? myThesDatFile;
	private int processors;

	public string? HunspellAffFile
	{
		get => this.hunspellAffFile;
		set
		{
			string fullPath = Path.GetFullPath(value ?? "");
			this.hunspellAffFile = File.Exists(fullPath) ? fullPath : throw new FileNotFoundException("Hunspell Aff file not found: " + fullPath);
		}
	}

	public string? HunspellDictFile
	{
		get => this.hunspellDictFile;
		set
		{
			string fullPath = Path.GetFullPath(value ?? "");
			this.hunspellDictFile = File.Exists(fullPath) ? fullPath : throw new FileNotFoundException("Hunspell Dict file not found: " + fullPath);
		}
	}

	public string? HunspellKey { get; set; }

	public string? HyphenDictFile
	{
		get => this.hyphenDictFile;
		set
		{
			string fullPath = Path.GetFullPath(value ?? "");
			this.hyphenDictFile = File.Exists(fullPath) ? fullPath : throw new FileNotFoundException("Hyphen Dict file not found: " + fullPath);
		}
	}

	public string? LanguageCode
	{
		get => this.languageCode;
		set
		{
			if(value is null)
				throw new ArgumentNullException("LanguageCode cannot be null");
			this.languageCode = !(value == string.Empty) ? value.ToLower() : throw new ArgumentException("LanguageCode cannot be empty");
		}
	}

	public string? MyThesDatFile
	{
		get => this.myThesDatFile;
		set
		{
			string fullPath = Path.GetFullPath(value ?? "");
			this.myThesDatFile = File.Exists(fullPath) ? fullPath : throw new FileNotFoundException("MyThes Dat file not found: " + fullPath);
		}
	}

	public int Processors
	{
		get => this.processors;
		set => this.processors = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(Processors), "Processors must be greater than 0");
	}
}
