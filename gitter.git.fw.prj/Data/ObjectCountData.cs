#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.AccessLayer
{
	public sealed class ObjectCountData
	{
		#region Data

		private readonly int _count;
		private readonly int _size;
		private readonly int _inPack;
		private readonly int _packs;
		private readonly int _sizePack;
		private readonly int _prunePackable;
		private readonly int _garbage;

		#endregion

		#region .ctor

		public ObjectCountData(int count, int size, int inPack, int packs, int sizePack, int prunePackable, int garbage)
		{
			_count = count;
			_size = size;
			_inPack = inPack;
			_packs = packs;
			_sizePack = sizePack;
			_prunePackable = prunePackable;
			_garbage = garbage;
		}

		#endregion

		#region Properties

		public int Count
		{
			get { return _count; }
		}

		public int Size
		{
			get { return _size; }
		}

		public int InPack
		{
			get { return _inPack; }
		}

		public int Packs
		{
			get { return _packs; }
		}

		public int PrunePackable
		{
			get { return _prunePackable; }
		}

		public int Garbage
		{
			get { return _garbage; }
		}

		#endregion
	}
}
