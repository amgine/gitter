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
	private readonly int processors;
	private volatile bool disposed;
	private Semaphore hunspellSemaphore;
	private Stack<Hunspell> hunspells;
	private object hunspellsLock = new();
	private Semaphore hyphenSemaphore;
	private Stack<Hyphen> hyphens;
	private readonly object hyphensLock = new();
	private Semaphore myThesSemaphore;
	private Stack<MyThes> myTheses;
	private readonly object myThesesLock = new();

	public bool Add(string word)
	{
		bool flag = false;
		lock(this.hunspellsLock)
		{
			foreach(Hunspell hunspell in this.hunspells)
				flag = hunspell.Add(word);
			return flag;
		}
	}

	public bool AddWithAffix(string word, string example)
	{
		bool flag = false;
		lock(this.hunspellsLock)
		{
			foreach(Hunspell hunspell in this.hunspells)
				flag = hunspell.AddWithAffix(word, example);
			return flag;
		}
	}

	public bool Remove(string word)
	{
		bool flag = false;
		lock(this.hunspellsLock)
		{
			foreach(Hunspell hunspell in this.hunspells)
				flag = hunspell.Remove(word);
			return flag;
		}
	}

	private Hunspell HunspellsPop()
	{
		if(this.IsDisposed)
			throw new ObjectDisposedException(nameof(SpellFactory));
		if(this.hunspells is null)
			throw new InvalidOperationException("Hunspell Dictionary isn't loaded");
		this.hunspellSemaphore.WaitOne();
		lock(this.hunspellsLock)
		{
			return this.hunspells.Pop();
		}
	}

	private void HunspellsPush(Hunspell hunspell)
	{
		lock(this.hunspellsLock)
			this.hunspells.Push(hunspell);
		this.hunspellSemaphore.Release();
	}

	private Hyphen HyphenPop()
	{
		if(this.IsDisposed)
			throw new ObjectDisposedException(nameof(SpellFactory));
		if(this.hyphens == null)
			throw new InvalidOperationException("Hyphen Dictionary isn't loaded");
		this.hyphenSemaphore.WaitOne();
		lock(this.hyphensLock)
			return this.hyphens.Pop();
	}

	private void HyphenPush(Hyphen hyphen)
	{
		lock(this.hyphensLock)
			this.hyphens.Push(hyphen);
		this.hyphenSemaphore.Release();
	}

	private MyThes MyThesPop()
	{
		if(this.IsDisposed)
			throw new ObjectDisposedException(nameof(SpellFactory));
		if(this.myTheses == null)
			throw new InvalidOperationException("MyThes Dictionary isn't loaded");
		this.myThesSemaphore.WaitOne();
		lock(this.myThesesLock)
			return this.myTheses.Pop();
	}

	private void MyThesPush(MyThes myThes)
	{
		lock(this.myThesesLock)
			this.myTheses.Push(myThes);
		this.myThesSemaphore.Release();
	}

	public SpellFactory(LanguageConfig config)
	{
		this.processors = config.Processors;
		if(!string.IsNullOrEmpty(config.HunspellAffFile))
		{
			this.hunspells = new Stack<Hunspell>(this.processors);
			for(int index = 0; index < this.processors; ++index)
			{
				if(config.HunspellKey != null && config.HunspellKey != string.Empty)
					this.hunspells.Push(new Hunspell(config.HunspellAffFile, config.HunspellDictFile, config.HunspellKey));
				else
					this.hunspells.Push(new Hunspell(config.HunspellAffFile, config.HunspellDictFile));
			}
		}
		if(!string.IsNullOrEmpty(config.HyphenDictFile))
		{
			this.hyphens = new Stack<Hyphen>(this.processors);
			for(int index = 0; index < this.processors; ++index)
				this.hyphens.Push(new Hyphen(config.HyphenDictFile));
		}
		if(!string.IsNullOrEmpty(config.MyThesDatFile))
		{
			this.myTheses = new Stack<MyThes>(this.processors);
			for(int index = 0; index < this.processors; ++index)
				this.myTheses.Push(new MyThes(config.MyThesDatFile));
		}
		this.hunspellSemaphore = new Semaphore(this.processors, this.processors);
		this.hyphenSemaphore = new Semaphore(this.processors, this.processors);
		this.myThesSemaphore = new Semaphore(this.processors, this.processors);
	}

	public bool IsDisposed => this.disposed;

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
		if(this.IsDisposed)
			return;
		this.disposed = true;
		for(int index = 0; index < this.processors; ++index)
			this.hunspellSemaphore.WaitOne();
		if(this.hunspells != null)
		{
			foreach(Hunspell hunspell in this.hunspells)
				hunspell.Dispose();
		}
		this.hunspellSemaphore.Release(this.processors);
		this.hunspellSemaphore = (Semaphore)null;
		this.hunspells = (Stack<Hunspell>)null;
		for(int index = 0; index < this.processors; ++index)
			this.hyphenSemaphore.WaitOne();
		if(this.hyphens != null)
		{
			foreach(Hyphen hyphen in this.hyphens)
				hyphen.Dispose();
		}
		this.hyphenSemaphore.Release(this.processors);
		this.hyphenSemaphore = (Semaphore)null;
		this.hyphens = (Stack<Hyphen>)null;
		for(int index = 0; index < this.processors; ++index)
			this.myThesSemaphore.WaitOne();
		this.myThesSemaphore.Release(this.processors);
		this.myThesSemaphore = (Semaphore)null;
		this.myTheses = (Stack<MyThes>)null;
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

	public HyphenResult Hyphenate(string word)
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

	public ThesResult LookupSynonyms(string word, bool useGeneration)
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
