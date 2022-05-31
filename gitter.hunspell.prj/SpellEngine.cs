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

namespace NHunspell;

public class SpellEngine : IDisposable
{
	private readonly object dictionaryLock;
	private Dictionary<string, SpellFactory> languages;
	private int processors;

	public SpellEngine()
	{
		processors     = Environment.ProcessorCount;
		languages      = new Dictionary<string, SpellFactory>();
		dictionaryLock = new object();
	}

	public bool IsDisposed
	{
		get
		{
			lock(dictionaryLock)
			{
				return languages is null;
			}
		}
	}

	public int Processors
	{
		get => processors;
		set => processors = value >= 1 ? value : throw new ArgumentOutOfRangeException(nameof(Processors), "Processors must be greater than 0");
	}

	public SpellFactory this[string language]
	{
		get
		{
			lock(dictionaryLock)
			{
				return languages[language.ToLower()];
			}
		}
	}

	public void AddLanguage(LanguageConfig config)
	{
		var lower = config.LanguageCode.ToLower();
		if(config.Processors < 1)
		{
			config.Processors = Processors;
		}
		var spellFactory = new SpellFactory(config);
		lock(dictionaryLock)
		{
			languages.Add(lower, spellFactory);
		}
	}

	public void Dispose()
	{
		if(IsDisposed) return;

		lock(dictionaryLock)
		{
			foreach(var spellFactory in languages.Values)
			{
				spellFactory.Dispose();
			}
			languages = default;
		}
	}
}
