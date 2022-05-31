﻿#region Copyright Notice
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

namespace gitter.Git.AccessLayer;

public sealed class ObjectCountData
{
	public ObjectCountData(int count, int size, int inPack, int packs, int sizePack, int prunePackable, int garbage)
	{
		Count = count;
		Size = size;
		InPack = inPack;
		Packs = packs;
		SizePack = sizePack;
		PrunePackable = prunePackable;
		Garbage = garbage;
	}

	public int Count { get; }

	public int Size { get; }

	public int InPack { get; }

	public int Packs { get; }

	public int SizePack { get; }

	public int PrunePackable { get; }

	public int Garbage { get; }
}
