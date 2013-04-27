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

namespace gitter.Git.Gui
{
	using System;

	/// <summary>Atomar graph position. Contains <see cref="GraphElement"/> &amp; their colors.</summary>
	/// <remarks>Graph consists of list of arrays of atoms.</remarks>
	public struct GraphAtom
	{
		public GraphElement Elements;
		public int[] ElementColors;

		public void Paint(GraphElement element, int color)
		{
			Elements |= element;
			if(element != GraphElement.Space)
			{
				if(ElementColors == null) ElementColors = new int[13];
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

		public bool IsEmpty
		{
			get { return Elements == GraphElement.Space; }
		}

		public bool HasElement(GraphElement element)
		{
			return (Elements & element) == element;
		}

		public void Paint(int elementid, int color)
		{
			Elements |= (GraphElement)(1<<elementid);
			if(elementid != 0)
			{
				if(ElementColors == null) ElementColors = new int[12];
				ElementColors[elementid] = color;
			}
		}

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
}
