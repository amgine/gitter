namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;

	public sealed class DefaultGraphBuilderFactory : IGraphBuilderFactory
	{
		public IGraphBuilder<T> CreateGraphBuilder<T>()
			where T : class
		{
			return new DefaultGraphBuilder<T>();
		}
	}

	internal sealed class DefaultGraphBuilder<T> : IGraphBuilder<T>
		where T : class
	{
		private static int ColorLookup(T[] line, int lineCount, T parent, int[] linecolors, IGraphColorProvider cprov)
		{
			for(int i = 0; i < lineCount; ++i)
			{
				if(line[i] == parent) return linecolors[i];
			}
			return cprov.AcquireColor();
		}

		private static void ContinueVerticalLines(GraphAtom[] graphLine, T[] line, int[] linecolors, int from, int to)
		{
			for(int i = from; i <= to; ++i)
			{
				if(line[i] != null)
				{
					graphLine[i].Paint(GraphElement.Vertical, linecolors[i]);
				}
			}
		}

		private static void DrawBottomConnection(GraphAtom[] graphLine, int color, int from, int to)
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

		private static GraphAtom[] BuildGraphLine0Child(IList<T> parents, T[] line, int[] linecolors, IGraphColorProvider cprov, ref int lineCount)
		{
			GraphAtom[] res;
			int lineColor = 0;
			// find position to insert dot
			int j = lineCount;
			while((j != 0) && (line[j - 1] == null))
			{
				--j;
			}
			// allocate memory for graph line
			switch(parents.Count)
			{
				case 0:
					// just a dot, generates no topology
					res = new GraphAtom[j + 1];
					// paint dot
					res[j].Paint(GraphElement.Dot, 0);
					break;
				case 1:
					// allocate color
					bool colorFound = false;
					int spaceId = -1;
					for(int i = 0; i < lineCount; ++i)
					{
						if(line[i] == parents[0] && !colorFound)
						{
							lineColor = linecolors[i];
							colorFound = true;
							if(spaceId != -1) break;
						}
						if(line[i] == null && spaceId == -1)
						{
							spaceId = i;
							if(colorFound) break;
						}
					}
					if(!colorFound)
					{
						lineColor = cprov.AcquireColor();
					}
					if(spaceId == -1)
					{
						// generate 1 new line
						lineCount = j + 1;
						res = new GraphAtom[lineCount];
					}
					else
					{
						j = spaceId;
						res = new GraphAtom[lineCount];
						ContinueVerticalLines(res, line, linecolors, j + 1, lineCount - 1);
					}
					// paint dot
					res[j].Paint(GraphElement.Dot, 0);
					line[j] = parents[0];
					linecolors[j] = lineColor;
					res[j].Paint(GraphElement.VerticalBottom, lineColor);
					break;
				default:
					var d = new Dictionary<int, int>(parents.Count);
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
						res = new GraphAtom[j + 1];
					}
					else
					{
						res = new GraphAtom[j + 1 + parents.Count - d.Count - 1];
					}
					// paint dot
					res[j].Paint(GraphElement.Dot, 0);
					// paint lines for parents
					int newLineId = j;
					for(int i = 0; i < parents.Count; ++i)
					{
						int to;
						if(d.TryGetValue(i, out to))
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
					break;
			}
			// continue lines from previos graph lines, preserving color
			ContinueVerticalLines(res, line, linecolors, 0, j - 1);
			return res;
		}

		private static GraphAtom[] BuildGraphLine(IList<T> parents, T[] line, int[] linecolors, IGraphColorProvider cprov, IList<int> pos, int id, ref int lineCount)
		{
			GraphAtom[] res;
			int j = pos[id];
			switch(parents.Count)
			{
				case 0:
					#region Line stops here
					{
						// allocate memory for new line
						res = new GraphAtom[lineCount];
						// paint
						// ...
						// .+.
						// ...
						res[j].Paint(GraphElement.Dot, 0);
						for(int i = 0; i < pos.Count; ++i)
						{
							line[pos[i]] = null;
						}
						cprov.ReleaseColor(linecolors[j]);
						if(j == lineCount - 1) // reduce graph width if it was last line
						{
							while(lineCount != 0 && line[lineCount - 1] == null)
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
						int lineColor;
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
						res = new GraphAtom[lineCount];
						// paint
						res[j].Paint(GraphElement.Dot, 0);
						res[j].Paint(GraphElement.VerticalBottom, lineColor);
						// continue lines from previous graph line
						ContinueVerticalLines(res, line, linecolors, 0, j - 1);
						ContinueVerticalLines(res, line, linecolors, j + 1, lineCount - 1);
					}
					#endregion
					break;
				default:
					#region Several lines start here (point where 1+ git branches were merged)
					{
						line[j] = null;
						bool lastItem = j == lineCount - 1;
						var lineColor = 0;
						Dictionary<T, int> unmergeable = null;
						var mergeable = new Dictionary<T, List<int>>(parents.Count);
						// find mergeable parents
						for(int i = 0; i < lineCount; ++i)
						{
							var p = line[i];
							if(p != null)
							{
								bool found = false;
								for(int c = 0; c < parents.Count; ++c)
								{
									if(p == parents[c])
									{
										found = true;
										List<int> positions;
										if(!mergeable.TryGetValue(p, out positions))
										{
											positions = new List<int>(parents.Count);
											mergeable.Add(p, positions);
										}
										positions.Add(i);
										break;
									}
								}
								if(found)
								{
									if(mergeable.Count == parents.Count) break;
								}
							}
						}
						if(parents.Count == mergeable.Count)
						{
							T closest = default(T);
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
									while(line[np] != null && !pos.Contains(np))
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
						// allocate memory for new line
						res = new GraphAtom[lineCount];
						// paint dot
						res[j].Paint(GraphElement.Dot, 0);
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
						// continue lines from previous graph line
						ContinueVerticalLines(res, line, linecolors, 0, lineCount - 1);
						// paint unmergeable
						int oldcolor = 0;
						if(unmergeable != null)
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

		private static void BuildUpperConnections(GraphAtom[] res, int[] linecolors, IGraphColorProvider cprov, IList<int> pos, int id)
		{
			int j = pos[id];
			for(int i = 0; i < pos.Count; ++i)
			{
				int cpos = pos[i];
				int lineColor = linecolors[i];
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

		public GraphAtom[][] BuildGraph(IList<T> items, Func<T, IList<T>> getParents)
		{
			Verify.Argument.IsNotNull(items, "items");
			Verify.Argument.IsNotNull(getParents, "getParents");

			var res = new GraphAtom[items.Count][];

			var cprov = new GraphColorProvider();

			int id = 0;
			var pos = new List<int>(items.Count);
			var line = new T[items.Count];
			var linecolors = new int[items.Count];
			var lineCount = 0;
			int[] upperLinecolors1 = new int[1];
			int[] upperLinecolors = null;
			foreach(var item in items)
			{
				var parents = getParents(item);
				pos.Clear();

				int p = -1;
				while(true)
				{
					p = Array.IndexOf<T>(line, item, p + 1);
					if(p != -1)
					{
						pos.Add(p);
					}
					else
					{
						break;
					}
				}

				GraphAtom[] graphLine;
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
							upperLinecolors1[0] = linecolors[pos[0]];
							graphLine = BuildGraphLine(parents, line, linecolors, cprov, pos, 0, ref lineCount);
							BuildUpperConnections(graphLine, upperLinecolors1, cprov, pos, 0);
						}
						break;
					default:
						{
							int pid = 0;
							//int pid = (pos.Count - 1) / 2; - attempt to build balanced graph, not good one
							if(upperLinecolors == null || upperLinecolors.Length < pos.Count)
							{
								upperLinecolors = new int[pos.Count];
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
				res[id] = graphLine;

				while(lineCount != 0 && (lineCount >= line.Length || line[lineCount] != null))
				{
					--lineCount;
				}

				++id;
			}

			return res;
		}

		public void CleanGraph(GraphAtom[] graph)
		{
			for(int i = 0; i < graph.Length; ++i)
			{
				if((graph[i].Elements & GraphElement.Dot) == GraphElement.Dot)
				{
					graph[i].Erase(GraphElement.VerticalTop);
					break;
				}
			}
		}

		public void CleanGraph(GraphAtom[] prev, GraphAtom[] next)
		{
			for(int i = 0; i < next.Length; ++i)
			{
				if((next[i].Elements & GraphElement.Dot) == GraphElement.Dot)
				{
					if(i < prev.Length)
					{
						if(((prev[i].Elements & (GraphElement.LeftBottomCorner | GraphElement.RightBottomCorner | GraphElement.VerticalBottom)) == GraphElement.Space) &&
						   ((next[i].Elements & GraphElement.VerticalTop) != GraphElement.Space))
						{
							next[i].Erase(GraphElement.VerticalTop);
							break;
						}
					}
					else
					{
						next[i].Erase(GraphElement.VerticalTop);
						break;
					}
				}
			}
		}

		public GraphAtom[] AddGraphLineToTop(GraphAtom[] next)
		{
			int id0 = 0;
			int id = 0;
			int color = 0;
			int len = 1;
			bool move = false;
			if(next != null)
			{
				for(int i = next.Length - 1; i >=0; --i)
				{
					if(next[i].Elements == GraphElement.Space)
						continue;
					if((next[i].Elements & (GraphElement.VerticalTop | GraphElement.LeftTopCorner | GraphElement.RightTopCorner | GraphElement.Dot)) != GraphElement.Space)
					{
						if(len == 1) len = i + 1;
					}
					if((next[i].Elements & GraphElement.Dot) == GraphElement.Dot)
					{
						id0 = id = i;
						break;
					}
				}
				if(next[id0].ElementColors != null)
				{
					if((next[id0].Elements & GraphElement.VerticalTop) == GraphElement.VerticalTop)
					{
						color = next[id0].ElementColors[GraphElementId.VerticalTop];
						move = true;
					}
					else
					{
						if((next[id0].Elements & GraphElement.VerticalBottom) == GraphElement.VerticalBottom)
						{
							color = next[id0].ElementColors[GraphElementId.VerticalBottom];
						}
						else
						{
							color = next[id0].ElementColors[GraphElementId.HorizontalLeft];
						}
					}
				}
				if(move)
				{
					++len;
					id = len - 1;
				}
			}
			var res = new GraphAtom[len];
			if(next != null)
			{
				for(int i = 0; i < next.Length; ++i)
				{
					if((next[i].Elements & GraphElement.VerticalTop) == GraphElement.VerticalTop)
					{
						int c = next[i].ElementColors[GraphElementId.VerticalTop];
						res[i].Paint(GraphElement.Vertical, c);
					}
					else if((next[i].Elements & GraphElement.LeftTopCorner) == GraphElement.LeftTopCorner)
					{
						int c = next[i].ElementColors[GraphElementId.LeftTopCorner];
						res[i].Paint(GraphElement.Vertical, c);
					}
					else if((next[i].Elements & GraphElement.RightTopCorner) == GraphElement.RightTopCorner)
					{
						int c = next[i].ElementColors[GraphElementId.RightTopCorner];
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
			if(next != null)
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
}
