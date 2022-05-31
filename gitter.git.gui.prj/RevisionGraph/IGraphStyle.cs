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
using System.Drawing;

using gitter.Framework;

/// <summary>Graph painter.</summary>
public interface IGraphStyle
{
	event EventHandler Changed;


	void DrawBackground(Graphics graphics, Dpi dpi, GraphCell[] graphLine, Rectangle bounds, Rectangle clip, int cellWidth, bool useColors);

	void DrawGraph(Graphics graphics, Dpi dpi, GraphCell[] graphLine, Rectangle bounds, Rectangle clip, int cellWidth, RevisionGraphItemType type, bool useColors);

	void DrawReferenceConnector(Graphics graphics, Dpi dpi, GraphCell[] graphLine, int graphX, int cellWidth, int refX, int y, int h);

	void DrawReferencePresenceIndicator(Graphics graphics, Dpi dpi, GraphCell[] graphLine, int graphX, int cellWidth, int y, int h);


	Rectangle DrawTag(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, Tag tag);

	Rectangle DrawBranch(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, BranchBase branch);

	Rectangle DrawStash(Graphics graphics, Dpi dpi, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, StashedState stash);


	bool HitTestReference(Rectangle bounds, int x, int y);


	int MeasureTag(Graphics graphics, Dpi dpi, Font font, StringFormat format, Tag tag);

	int MeasureBranch(Graphics graphics, Dpi dpi, Font font, StringFormat format, Branch branch);

	int MeasureStash(Graphics graphics, Dpi dpi, Font font, StringFormat format, StashedState stash);
}
