#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework.Services
{
	using System;
	using System.Collections.Generic;

	sealed class SpellcheckerCache
	{
		private readonly Queue<string> _queue;
		private readonly Dictionary<string, bool> _results;
		private readonly int _maxCount;

		public SpellcheckerCache(int maxCount = 5000)
		{
			Verify.Argument.IsNotNegative(maxCount, nameof(maxCount));

			_maxCount = maxCount;
			_queue    = new(capacity: maxCount + 1);
			_results  = new(capacity: maxCount + 1);
		}

		public bool TryGetResult(string word, out bool result)
			=> _results.TryGetValue(word, out result);

		public void CacheResult(string word, bool result)
		{
			_results.Add(word, result);
			_queue.Enqueue(word);
			Prune();
		}

		private void Prune()
		{
			while(_queue.Count > _maxCount)
			{
				_results.Remove(_queue.Dequeue());
			}
		}
	}
}
