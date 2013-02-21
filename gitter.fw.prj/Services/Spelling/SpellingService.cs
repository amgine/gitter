namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Text;
	using System.Xml;
	using System.IO;
	using System.Threading;

	using NHunspell;

	using gitter.Framework.Configuration;

	public static class SpellingService
	{
		#region Data

		private static readonly Dictionary<string, Hunspell> _spellers;
		private static readonly string DictionaryPath;
		private static int _loadCounter;
		private static readonly ManualResetEvent _evtDictionariesReady;

		#endregion

		static SpellingService()
		{
			var asmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Hunspell.NativeDllPath = Path.Combine(asmPath, "Hunspell");
			DictionaryPath = Path.Combine(asmPath, "Dictionaries");
			_spellers = new Dictionary<string, Hunspell>();
			_evtDictionariesReady = new ManualResetEvent(true);
		}

		public static ICollection<string> GetAvailableLocales()
		{
			var list = new List<string>();
			try
			{
				if(Directory.Exists(DictionaryPath))
				{
					foreach(var file in Directory.EnumerateFiles(DictionaryPath, "*.aff", SearchOption.TopDirectoryOnly))
					{
						if(File.Exists(file.Substring(0, file.Length - 4) + ".dic"))
						{
							list.Add(Path.GetFileNameWithoutExtension(file));
						}
					}
				}
			}
			catch(DirectoryNotFoundException)
			{
			}
			catch(FileNotFoundException)
			{
			}
			return list;
		}

		public static ICollection<string> GetLoadedLocales()
		{
			return _spellers.Keys;
		}

		public static void LoadLocale(string localeName)
		{
			var aff = Path.Combine(DictionaryPath, localeName + ".aff");
			var dic = Path.Combine(DictionaryPath, localeName + ".dic");

			Hunspell h;
			try
			{
				h = new Hunspell(aff, dic);
			}
			catch
			{
				h = null;
			}
			if(h != null)
			{
				lock(_spellers)
				{
					_spellers.Add(localeName, h);
				}
			}
		}

		private static void LoadLocaleAsync(string localeName)
		{
			Action<string> action = LoadLocale;
			action.BeginInvoke(localeName, OnDictionaryLoaded, null);
		}

		private static void OnDictionaryLoaded(IAsyncResult ar)
		{
			if(Interlocked.Decrement(ref _loadCounter) == 0) _evtDictionariesReady.Set();
		}

		public static bool IsLoaded(string localeName)
		{
			return _spellers.ContainsKey(localeName);
		}

		public static void UnloadLocale(string localeName)
		{
			_evtDictionariesReady.WaitOne();
			Hunspell speller;
			if(_spellers.TryGetValue(localeName, out speller))
			{
				speller.Dispose();
				_spellers.Remove(localeName);
			}
		}

		public static bool Spell(string word)
		{
			_evtDictionariesReady.WaitOne();
			lock(_spellers)
			{
				foreach(var speller in _spellers.Values)
					if(speller.Spell(word)) return true;
			}
			return false;
		}

		public static bool Enabled
		{
			get
			{
				_evtDictionariesReady.WaitOne();
				lock(_spellers) return _spellers.Count != 0;
			}
		}

		public static void SaveTo(Section section)
		{
			_evtDictionariesReady.WaitOne();
			section.ClearParameters();
			lock(_spellers)
			{
				foreach(var locale in _spellers.Keys)
				{
					section.SetValue(locale, true);
				}
			}
		}

		public static void LoadFrom(Section section)
		{
			if(section.ParameterCount != 0)
			{
				_loadCounter = section.ParameterCount;
				_evtDictionariesReady.Reset();
				foreach(var p in section.Parameters)
				{
					if(section.GetValue(p.Name, false))
					{
						LoadLocaleAsync(p.Name);
					}
					else
					{
						if(Interlocked.Decrement(ref _loadCounter) == 0)
						{
							_evtDictionariesReady.Set();
						}
					}
				}
			}
		}
	}
}
