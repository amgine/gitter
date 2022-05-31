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

namespace gitter.Git.Gui;

using System;

/// <summary>Graph cell. Contains <see cref="GraphElement"/>s &amp; their indexed colors.</summary>
public struct GraphCell
{
	public GraphElement Elements;
	private int[] ElementColors;

	public void Paint(GraphElement element, int color)
	{
		Elements |= element;
		if(element != GraphElement.Space)
		{
			ElementColors ??= new int[13];
			int pos = (int)element;
			int offset = 0;
			while(pos != 0)
			{
				if((pos & 1) != 0) ElementColors[offset] = color;
				pos >>= 1;
				++offset;
			}
		}
	}

	public int ColorOf(int elementId)
		=> ElementColors is not null ? ElementColors[elementId] : default;

	public bool IsEmpty => Elements == GraphElement.Space;

	public bool HasAnyOfElements(GraphElement elements)
		=> (Elements & elements) != GraphElement.Space;

	public bool HasElement(GraphElement element)
		=> (Elements & element) == element;

	public void Erase(GraphElement element)
	{
		Elements &= ~element;
		if(Elements == GraphElement.Space)
			ElementColors = null;
	}

	public void Erase()
	{
		Elements = GraphElement.Space;
		ElementColors = null;
	}
}
