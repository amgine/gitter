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
using System.Threading;

namespace NHunspell;

public class SpellFactory : IDisposable
{
	private readonly int _processors;
	private Semaphore _hunspellSemaphore;
	private Stack<Hunspell> _hunspells = default!;
	private object hunspellsLock = new();
	private Semaphore _hyphenSemaphore;
	private Stack<Hyphen> _hyphens = default!;
	private readonly object hyphensLock = new();
	private Semaphore _myThesSemaphore;
	private Stack<MyThes> myTheses = default!;
	private readonly object myThesesLock = new();

	public bool Add(string word)
	{
		bool flag = false;
		lock(this.hunspellsLock)
		{
			foreach(Hunspell hunspell in this._hunspells)
				flag = hunspell.Add(word);
			return flag;
		}
	}

	public bool AddWithAffix(string word, string example)
	{
		bool flag = false;
		lock(this.hunspellsLock)
		{
			foreach(Hunspell hunspell in this._hunspells)
				flag = hunspell.AddWithAffix(word, example);
			return flag;
		}
	}

	public bool Remove(string word)
	{
		bool flag = false;
		lock(this.hunspellsLock)
		{
			foreach(Hunspell hunspell in this._hunspells)
				flag = hunspell.Remove(word);
			return flag;
		}
	}

	private Hunspell HunspellsPop()
	{
		if(this.IsDisposed)
			throw new ObjectDisposedException(nameof(SpellFactory));
		if(this._hunspells is null)
			throw new InvalidOperationException("Hunspell Dictionary isn't loaded");
		this._hunspellSemaphore.WaitOne();
		lock(this.hunspellsLock)
		{
			return this._hunspells.Pop();
		}
	}

	private void HunspellsPush(Hunspell hunspell)
	{
		lock(this.hunspellsLock)
			this._hunspells.Push(hunspell);
		this._hunspellSemaphore.Release();
	}

	private Hyphen HyphenPop()
	{
		if(this.IsDisposed)
			throw new ObjectDisposedException(nameof(SpellFactory));
		if(this._hyphens == null)
			throw new InvalidOperationException("Hyphen Dictionary isn't loaded");
		this._hyphenSemaphore.WaitOne();
		lock(this.hyphensLock)
			return this._hyphens.Pop();
	}

	private void HyphenPush(Hyphen hyphen)
	{
		lock(this.hyphensLock)
			this._hyphens.Push(hyphen);
		this._hyphenSemaphore.Release();
	}

	private MyThes MyThesPop()
	{
		if(this.IsDisposed)
			throw new ObjectDisposedException(nameof(SpellFactory));
		if(this.myTheses == null)
			throw new InvalidOperationException("MyThes Dictionary isn't loaded");
		this._myThesSemaphore.WaitOne();
		lock(this.myThesesLock)
			return this.myTheses.Pop();
	}

	private void MyThesPush(MyThes myThes)
	{
		lock(myThesesLock)
			this.myTheses.Push(myThes);
		_myThesSemaphore.Release();
	}

	public SpellFactory(LanguageConfig config)
	{
		_processors = config.Processors;
		if(config.HunspellAffFile is { Length: not 0 } affFile && config.HunspellDictFile is { Length: not 0 } dictFile)
		{
			_hunspells = new Stack<Hunspell>(capacity: _processors);
			for(int index = 0; index < _processors; ++index)
			{
				var hunspell = config.HunspellKey is { Length: not 0 } key
					? new Hunspell(affFile, dictFile, key)
					: new Hunspell(affFile, dictFile);
				_hunspells.Push(hunspell);
			}
		}
		if(config.HyphenDictFile is { Length: not 0 } hyphenDict)
		{
			_hyphens = new Stack<Hyphen>(capacity: _processors);
			for(int index = 0; index < _processors; ++index)
			{
				_hyphens.Push(new Hyphen(hyphenDict));
			}
		}
		if(config.MyThesDatFile is { Length: not 0 } thesFile)
		{
			myTheses = new Stack<MyThes>(_processors);
			for(int index = 0; index < _processors; ++index)
			{
				myTheses.Push(new MyThes(thesFile));
			}
		}
		_hunspellSemaphore = new(_processors, _processors);
		_hyphenSemaphore   = new(_processors, _processors);
		_myThesSemaphore   = new(_processors, _processors);
	}

	public bool IsDisposed { get; private set; }

	public List<string> Analyze(string word)
	{
		var hunspell = HunspellsPop();
		try
		{
			return hunspell.Analyze(word);
		}
		finally
		{
			HunspellsPush(hunspell);
		}
	}

	public void Dispose()
	{
		if(IsDisposed) return;

		IsDisposed = true;
		for(int index = 0; index < this._processors; ++index)
		{
			_hunspellSemaphore.WaitOne();
		}
		if(_hunspells is not null)
		{
			foreach(var hunspell in _hunspells)
			{
				hunspell.Dispose();
			}
		}
		this._hunspellSemaphore.Release(this._processors);
		this._hunspellSemaphore = default!;
		this._hunspells = default!;
		for(int index = 0; index < _processors; ++index)
		{
			_hyphenSemaphore.WaitOne();
		}
		if(_hyphens is not null)
		{
			foreach(var hyphen in _hyphens)
			{
				hyphen.Dispose();
			}
		}
		_hyphenSemaphore.Release(this._processors);
		_hyphenSemaphore = default!;
		_hyphens = default!;
		for(int index = 0; index < this._processors; ++index)
		{
			_myThesSemaphore.WaitOne();
		}
		_myThesSemaphore.Release(_processors);
		_myThesSemaphore = default!;
		myTheses = default!;
	}

	public List<string> Generate(string word, string sample)
	{
		var hunspell = HunspellsPop();
		try
		{
			return hunspell.Generate(word, sample);
		}
		finally
		{
			HunspellsPush(hunspell);
		}
	}

	public HyphenResult? Hyphenate(string word)
	{
		var hyphen = HyphenPop();
		try
		{
			return hyphen.Hyphenate(word);
		}
		finally
		{
			HyphenPush(hyphen);
		}
	}

	public ThesResult? LookupSynonyms(string word, bool useGeneration)
	{
		var myThes   = default(MyThes);
		var hunspell = default(Hunspell);
		try
		{
			myThes = MyThesPop();
			if(!useGeneration)
			{
				return myThes.Lookup(word);
			}
			hunspell = HunspellsPop();
			return myThes.Lookup(word, hunspell);
		}
		finally
		{
			if(myThes is not null)
			{
				MyThesPush(myThes);
			}
			if(hunspell is not null)
			{
				HunspellsPush(hunspell);
			}
		}
	}

	public bool Spell(string word)
	{
		var hunspell = HunspellsPop();
		try
		{
			return hunspell.Spell(word);
		}
		finally
		{
			HunspellsPush(hunspell);
		}
	}

	public List<string> Stem(string word)
	{
		var hunspell = HunspellsPop();
		try
		{
			return hunspell.Stem(word);
		}
		finally
		{
			HunspellsPush(hunspell);
		}
	}

	public List<string> Suggest(string word)
	{
		var hunspell = HunspellsPop();
		try
		{
			return hunspell.Suggest(word);
		}
		finally
		{
			HunspellsPush(hunspell);
		}
	}
}
