﻿#region Copyright Notice
	using gitter.Framework;

				Preallocated<DiffColumnHeader>.EmptyArray,
						Preallocated<DiffLineState>.EmptyArray,
						Preallocated<int>.EmptyArray,
				lines.Add(new DiffLine(DiffLineState.Header, Preallocated<DiffLineState>.EmptyArray, Preallocated<int>.EmptyArray, ReadLine()));
				lines.Add(new DiffLine(state, Preallocated<DiffLineState>.EmptyArray, Preallocated<int>.EmptyArray, ReadLine()));
			return new DiffHunk(Preallocated<DiffColumnHeader>.EmptyArray, lines, new DiffStats(0, 0, lines.Count - headers, headers), true);