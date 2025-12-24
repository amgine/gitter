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
using System.Collections.Generic;

using gitter.Framework;

using ColorIndex = short;

public sealed class DefaultGraphBuilderFactory : IGraphBuilderFactory
{
	public IGraphBuilder<T> CreateGraphBuilder<T>() where T : class
		=> new GraphBuilder<T>();
}

internal sealed class GraphBuilder<T> : IGraphBuilder<T>
	where T : class
{
	private static int ColorLookup(T?[] line, int lineCount, T parent, ColorIndex[] linecolors, IGraphColorProvider cprov)
	{
		for(int i = 0; i < lineCount; ++i)
		{
			if(line[i] == parent) return linecolors[i];
		}
		return cprov.AcquireColor();
	}

	private static void ContinueVerticalLines(GraphCell[] graphLine, T?[] line, ColorIndex[] linecolors, int from, int to)
	{
		for(int i = from; i <= to; ++i)
		{
			if(line[i] is not null)
			{
				graphLine[i].Paint(GraphElement.Vertical, linecolors[i]);
			}
		}
	}

	private static void DrawBottomConnection(GraphCell[] graphLine, ColorIndex color, int from, int to)
	{
		if(to > from)
		{
			// draw something like
			// ... ... .|. ...
			//  j       i
			// ... ... ... ...
			// .+- --- ... ...
			// ... ... \.. ...
			graphLine[from].Paint(GraphElement.HorizontalRight, color);
			for(int k = from + 1; k < to; ++k)
			{
				graphLine[k].Paint(GraphElement.Horizontal, color);
			}
			graphLine[to].Paint(GraphElement.LeftBottomCorner, color);
		}
		else if(to < from)
		{
			// draw something like
			// .|. ... ... ...
			//  i       j
			// ... ... ... ...
			// ... --- -+. ...
			// ../ ... ... ...
			graphLine[from].Paint(GraphElement.HorizontalLeft, color);
			for(int k = from - 1; k > to; --k)
			{
				graphLine[k].Paint(GraphElement.Horizontal, color);
			}
			graphLine[to].Paint(GraphElement.RightBottomCorner, color);
		}
		else
		{
			// i,j
			// ...
			// .+.
			// .|.
			graphLine[from].Paint(GraphElement.VerticalBottom, color);
		}
	}

	private static GraphCell[] BuildGraphLine0Child(Many<T> parents, T?[] line, ColorIndex[] linecolors, IGraphColorProvider cprov, ref int lineCount)
	{
		GraphCell[] res;
		ColorIndex lineColor;
		// find position to insert dot
		int j = lineCount;
		while((j != 0) && (line[j - 1] is null))
		{
			--j;
		}
		// allocate memory for graph line
		switch(parents.Count)
		{
			case 0:
				// just a dot, generates no topology
				res = new GraphCell[j + 1];
				// paint dot
				var color = cprov.AcquireColor();
				res[j].Paint(GraphElement.Dot, color);
				cprov.ReleaseColor(color);
				break;
			case 1:
				int spaceId = -1;
				for(int i = 0; i < lineCount; ++i)
				{
					if(line[i] is null && spaceId == -1)
					{
						spaceId = i;
						break;
					}
				}
				lineColor = cprov.AcquireColor();
				if(spaceId == -1)
				{
					// generate 1 new line
					lineCount = j + 1;
					res = new GraphCell[lineCount];
				}
				else
				{
					j = spaceId;
					res = new GraphCell[lineCount];
					ContinueVerticalLines(res, line, linecolors, j + 1, lineCount - 1);
				}
				// paint dot
				res[j].Paint(GraphElement.Dot, lineColor);
				line[j] = parents.First();
				linecolors[j] = lineColor;
				res[j].Paint(GraphElement.VerticalBottom, lineColor);
				break;
			default:
				var d = new Dictionary<int, int>(capacity: parents.Count);
				for(int p = 0; p < parents.Count; ++p)
				{
					for(int i = 0; i < lineCount; ++i)
					{
						if(line[i] == parents[p])
						{
							d.Add(p, i);
							break;
						}
					}
				}
				// generate parents - 1 new lines
				lineCount = j + 1 + parents.Count - d.Count - 1;
				if(d.Count == parents.Count)
				{
					res = new GraphCell[j + 1];
				}
				else
				{
					res = new GraphCell[j + 1 + parents.Count - d.Count - 1];
				}
				// paint lines for parents
				int newLineId = j;
				for(int i = 0; i < parents.Count; ++i)
				{
					if(d.TryGetValue(i, out var to))
					{
						lineColor = linecolors[to];
						DrawBottomConnection(res, lineColor, j, to);
					}
					else
					{
						// something like
						// ... ... ...
						// .+- --- ...
						// .|. \.. \..
						line[newLineId] = parents[i];
						linecolors[newLineId] = lineColor = cprov.AcquireColor();
						DrawBottomConnection(res, lineColor, j, newLineId);
						newLineId++;
					}
				}
				// paint dot
				res[j].Paint(GraphElement.Dot, linecolors[j]);
				break;
		}
		// continue lines from previous graph lines, preserving color
		ContinueVerticalLines(res, line, linecolors, 0, j - 1);
		return res;
	}

	private static GraphCell[] BuildGraphLine(Many<T> parents, T?[] line, ColorIndex[] linecolors, IGraphColorProvider cprov, List<int> pos, int id, ref int lineCount)
	{
		GraphCell[] res;
		int j = pos[id];
		switch(parents.Count)
		{
			case 0:
				#region Line stops here
				{
					// allocate memory for new line
					res = new GraphCell[lineCount];
					// paint
					// ...
					// .+.
					// ...
					res[j].Paint(GraphElement.Dot, linecolors[j]);
					for(int i = 0; i < pos.Count; ++i)
					{
						line[pos[i]] = null;
					}
					cprov.ReleaseColor(linecolors[j]);
					if(j == lineCount - 1) // reduce graph width if it was last line
					{
						while(lineCount != 0 && line[lineCount - 1] is null)
						{
							--lineCount;
						}
					}
					// continue lines from previous graph line
					ContinueVerticalLines(res, line, linecolors, 0, lineCount - 1);
				}
				#endregion
				break;
			case 1:
				#region Line continues - most common case
				{
					ColorIndex lineColor;
					if(pos.Count > 1)
					{
						lineColor = cprov.AcquireColor();
						cprov.ReleaseColor(linecolors[j]);
						linecolors[j] = lineColor;
					}
					else
					{
						lineColor = linecolors[j];
					}
					for(int i = 0; i < pos.Count; ++i)
					{
						line[pos[i]] = null;
					}
					line[j] = parents[0];
					// allocate memory for new graph line
					res = new GraphCell[lineCount];
					// paint
					res[j].Paint(GraphElement.Dot, lineColor);
					res[j].Paint(GraphElement.VerticalBottom, lineColor);
					// continue lines from previous graph line
					ContinueVerticalLines(res, line, linecolors, 0, j - 1);
					ContinueVerticalLines(res, line, linecolors, j + 1, lineCount - 1);
				}
				#endregion
				break;
			default:
				#region 2 or more start here (point where 1+ git branches were merged)
				{
					line[j] = null;
					//bool lastItem = j == lineCount - 1;
					ColorIndex lineColor;
					var unmergeable = default(Dictionary<T, int>);
					var mergeable   = new Dictionary<T, List<int>>(parents.Count);
					// find mergeable parents
					for(int i = 0; i < lineCount; ++i)
					{
						var p = line[i];
						if(p is null) continue;
						if(parents.Contains(p))
						{
							if(!mergeable.TryGetValue(p, out var positions))
							{
								mergeable.Add(p, positions = new(parents.Count));
							}
							positions.Add(i);
							if(mergeable.Count == parents.Count) break;
						}
					}
					if(parents.Count == mergeable.Count)
					{
						var closest = default(T)!;
						int closestPosition = 0;
						int minD = int.MaxValue;
						bool found = true;
						foreach(var kvp in mergeable)
						{
							var positions = kvp.Value;
							for(int i = 0; i < positions.Count; ++i)
							{
								if(positions[i] <= j)
								{
									found = false;
									break;
								}
								else
								{
									var d = positions[i] - j;
									if(d > 0 && d < minD)
									{
										closestPosition = j;
										minD = d;
										closest = kvp.Key;
									}
								}
							}
						}
						if(found)
						{
							var list = mergeable[closest];
							if(list.Count == 1)
							{
								mergeable.Remove(closest);
							}
							else
							{
								list.Remove(closestPosition);
							}
							unmergeable = new Dictionary<T, int>();
							unmergeable.Add(closest, j);
						}
					}
					else
					{
						// fill unmergeable
						int np = j;
						unmergeable = new Dictionary<T, int>(parents.Count - mergeable.Count);
						for(int c = 0; c < parents.Count; ++c)
						{
							var p = parents[c];
							if(!mergeable.ContainsKey(p))
							{
								while(line[np] is not null && !pos.Contains(np))
								{
									++np;
								}
								unmergeable.Add(p, np);
								++np;
							}
						}
						if(np > lineCount)
						{
							lineCount = np;
						}
					}
					res = new GraphCell[lineCount];
					res[j].Paint(GraphElement.Dot, linecolors[j]);
					// paint mergeable
					foreach(var c in mergeable)
					{
						var positions = c.Value;
						int mind = int.MaxValue;
						int pid = 0;
						for(int p = 0; p < positions.Count; ++p)
						{
							int d = Math.Abs(j - positions[p]);
							if(d < mind)
							{
								mind = d;
								pid = p;
							}
						}
						int i = positions[pid];
						lineColor = linecolors[i];
						DrawBottomConnection(res, lineColor, j, i);
					}
					for(int i = 0; i < pos.Count; ++i)
					{
						line[pos[i]] = null;
					}
					ContinueVerticalLines(res, line, linecolors, 0, lineCount - 1);
					// paint unmergeable
					var oldcolor = default(ColorIndex);
					if(unmergeable is not null)
					{
						foreach(var c in unmergeable)
						{
							int i = c.Value;
							lineColor = cprov.AcquireColor();
							DrawBottomConnection(res, lineColor, j, i);
							if(i == j)
							{
								oldcolor = linecolors[i];
							}
							linecolors[i] = lineColor;
							line[i] = c.Key;
						}
					}
					cprov.ReleaseColor(oldcolor);
				}
				#endregion
				break;
		}
		return res;
	}

	private static void BuildUpperConnections(GraphCell[] res, ColorIndex[] linecolors, IGraphColorProvider cprov, List<int> pos, int id)
	{
		int j = pos[id];
		for(int i = 0; i < pos.Count; ++i)
		{
			int cpos = pos[i];
			var lineColor = linecolors[i];
			if(cpos < j)
			{
				res[cpos].Paint(GraphElement.RightTopCorner, lineColor);
				if(i != pos.Count - 1)
				{
					int next = pos[i + 1];
					if(next >= j)
					{
						next = j - 1;
						res[j].Paint(GraphElement.HorizontalLeft, lineColor);
					}
					for(int k = cpos + 1; k <= next; ++k)
					{
						res[k].Paint(GraphElement.Horizontal, lineColor);
					}
				}
				else
				{
					int next = j - 1;
					res[j].Paint(GraphElement.HorizontalLeft, lineColor);
					for(int k = cpos + 1; k <= next; ++k)
					{
						res[k].Paint(GraphElement.Horizontal, lineColor);
					}
				}
				cprov.ReleaseColor(lineColor);
			}
			else if(cpos > j)
			{
				res[cpos].Paint(GraphElement.LeftTopCorner, lineColor);
				if(i != 0)
				{
					int prev = pos[i - 1];
					if(prev <= j)
					{
						prev = j + 1;
						res[j].Paint(GraphElement.HorizontalRight, lineColor);
					}
					for(int k = prev; k < cpos; ++k)
					{
						res[k].Paint(GraphElement.Horizontal, lineColor);
					}
				}
				else
				{
					int prev = j + 1;
					res[j].Paint(GraphElement.HorizontalRight, lineColor);
					for(int k = prev; k < cpos; ++k)
					{
						res[k].Paint(GraphElement.Horizontal, lineColor);
					}
				}
				cprov.ReleaseColor(lineColor);
			}
			else
			{
				res[cpos].Paint(GraphElement.VerticalTop, lineColor);
			}
		}
	}

	public GraphCell[][] BuildGraph(IReadOnlyList<T> items, Func<T, Many<T>> getParents)
	{
		Verify.Argument.IsNotNull(items);
		Verify.Argument.IsNotNull(getParents);

		static void UpdatePositions(List<int> pos, T?[] line, T item)
		{
			pos.Clear();
			int p = -1;
			while(true)
			{
				p = Array.IndexOf(line, item, p + 1);
				if(p < 0) break;
				pos.Add(p);
			}
		}

		var res = new GraphCell[items.Count][];

		var cprov = new GraphColorProvider(GitterApplication.Style.Type == GitterStyleType.LightBackground
			? GraphColors.ColorsForLightBackground
			: GraphColors.ColorsForDarkBackground);

		int id = 0;
		var pos        = new List<int>(capacity: 8);
		var line       = new T?        [items.Count];
		var linecolors = new ColorIndex[items.Count];
		var lineCount  = 0;
		ColorIndex[]? upperLinecolors1 = default;
		ColorIndex[]? upperLinecolors  = null;
		foreach(var item in items)
		{
			UpdatePositions(pos, line, item);

			var parents = getParents(item);

			GraphCell[] graphLine;
			// how many lines can be merged in a dot?
			switch(pos.Count)
			{
				case 0:
					{
						graphLine = BuildGraphLine0Child(parents, line, linecolors, cprov, ref lineCount);
					}
					break;
				case 1:
					{
						upperLinecolors1 ??= new ColorIndex[1];
						upperLinecolors1[0] = linecolors[pos[0]];
						graphLine = BuildGraphLine(parents, line, linecolors, cprov, pos, 0, ref lineCount);
						BuildUpperConnections(graphLine, upperLinecolors1, cprov, pos, 0);
					}
					break;
				default:
					{
						int pid = 0;
						//int pid = (pos.Count - 1) / 2; - attempt to build balanced graph, not good one
						if(upperLinecolors is null || upperLinecolors.Length < pos.Count)
						{
							upperLinecolors = new ColorIndex[pos.Count];
						}
						for(int i = 0; i < pos.Count; ++i)
						{
							upperLinecolors[i] = linecolors[pos[i]];
						}
						graphLine = BuildGraphLine(parents, line, linecolors, cprov, pos, pid, ref lineCount);
						BuildUpperConnections(graphLine, upperLinecolors, cprov, pos, pid);
					}
					break;
			}
			res[id++] = graphLine;

			while(lineCount > 0 && (lineCount >= line.Length || line[lineCount] is not null))
			{
				--lineCount;
			}
		}

		return res;
	}

	public void CleanGraph(GraphCell[]? graph)
	{
		if(graph is null) return;

		for(int i = 0; i < graph.Length; ++i)
		{
			if(graph[i].HasElement(GraphElement.Dot))
			{
				graph[i].Erase(GraphElement.VerticalTop);
				break;
			}
		}
	}

	public void CleanGraph(GraphCell[]? prev, GraphCell[]? next)
	{
		if(prev is null) return;
		if(next is null) return;

		for(int i = 0; i < next.Length; ++i)
		{
			if(!next[i].HasElement(GraphElement.Dot)) continue;
			if(i >= prev.Length || !prev[i].HasAnyOfElements(GraphElement.LeftBottomCorner | GraphElement.RightBottomCorner | GraphElement.VerticalBottom))
			{
				next[i].Erase(GraphElement.VerticalTop);
			}
			break;
		}
	}

	public GraphCell[] AddGraphLineToTop(GraphCell[]? next)
	{
		int id0 = 0;
		int id = 0;
		var color = default(ColorIndex);
		int len = 1;
		bool move = false;
		if(next is not null)
		{
			for(int i = next.Length - 1; i >=0; --i)
			{
				if(next[i].IsEmpty) continue;

				if((next[i].Elements & (GraphElement.VerticalTop | GraphElement.LeftTopCorner | GraphElement.RightTopCorner | GraphElement.Dot)) != GraphElement.Space)
				{
					if(len == 1) len = i + 1;
				}
				if(next[i].HasElement(GraphElement.Dot))
				{
					id0 = id = i;
					break;
				}
			}
			if(!next[id0].IsEmpty)
			{
				if(next[id0].HasElement(GraphElement.VerticalTop))
				{
					color = next[id0].ColorOf(GraphElementId.VerticalTop);
					move = true;
				}
				else
				{
					color = next[id0].HasElement(GraphElement.VerticalBottom)
						? next[id0].ColorOf(GraphElementId.VerticalBottom)
						: next[id0].ColorOf(GraphElementId.HorizontalLeft);
				}
			}
			if(move)
			{
				++len;
				id = len - 1;
			}
		}
		var res = new GraphCell[len];
		if(next is not null)
		{
			for(int i = 0; i < next.Length; ++i)
			{
				if(next[i].HasElement(GraphElement.VerticalTop))
				{
					var c = next[i].ColorOf(GraphElementId.VerticalTop);
					res[i].Paint(GraphElement.Vertical, c);
				}
				else if(next[i].HasElement(GraphElement.LeftTopCorner))
				{
					var c = next[i].ColorOf(GraphElementId.LeftTopCorner);
					res[i].Paint(GraphElement.Vertical, c);
				}
				else if(next[i].HasElement(GraphElement.RightTopCorner))
				{
					var c = next[i].ColorOf(GraphElementId.RightTopCorner);
					res[i].Paint(GraphElement.Vertical, c);
				}
			}
		}
		if(move)
		{
			res[id0].Paint(GraphElement.RightBottomCorner, color);
			for(int i = id0 + 1; i < id; ++i)
			{
				res[i].Paint(GraphElement.Horizontal, color);
			}
			res[id].Paint(GraphElement.HorizontalLeft, color);
		}
		res[id].Paint(GraphElement.Dot, color);
		if(next is not null)
		{
			if(!move)
			{
				res[id].Paint(GraphElement.VerticalBottom, color);
			}
			next[id0].Paint(GraphElement.VerticalTop, color);
		}
		return res;
	}

	//public GraphAtom[] AddGraphLineToTop(GraphAtom[] topLine)
	//{
	//    int id = 0;
	//    int color = 0;
	//    if(topLine != null)
	//    {
	//        for(int i = 0; i < topLine.Length; ++i)
	//        {
	//            if((topLine[i].Elements & GraphElements.Dot) == GraphElements.Dot)
	//            {
	//                id = i;
	//                break;
	//            }
	//        }
	//        if(topLine[id].ElementColors != null)
	//            color = topLine[id].ElementColors[GraphElementId.VerticalBottom];
	//        topLine[id].Paint(GraphElements.VerticalTop, color);
	//    }
	//    var res = new GraphAtom[id + 1];
	//    res[id].Paint(GraphElements.Dot, 0);
	//    res[id].Paint(GraphElements.VerticalBottom, color);
	//    return res;
	//}
}
