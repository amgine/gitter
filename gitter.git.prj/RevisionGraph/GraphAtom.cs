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
