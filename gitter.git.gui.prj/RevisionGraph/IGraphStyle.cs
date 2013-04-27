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
	using System.Drawing;

	/// <summary>Graph painter.</summary>
	public interface IGraphStyle
	{
		void DrawGraph(Graphics graphics, GraphAtom[] graphLine, Rectangle bounds, int cellWidth, RevisionGraphItemType type, bool useColors);

		void DrawReferenceConnector(Graphics graphics, GraphAtom[] graphLine, int graphX, int cellWidth, int refX, int y, int h);

		void DrawReferencePresenceIndicator(Graphics graphics, GraphAtom[] graphLine, int graphX, int cellWidth, int y, int h);


		int DrawTag(Graphics graphics, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, Tag tag);

		int DrawBranch(Graphics graphics, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, BranchBase branch);

		int DrawStash(Graphics graphics, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, StashedState stash);


		int MeasureTag(Graphics graphics, Font font, StringFormat format, Tag tag);

		int MeasureBranch(Graphics graphics, Font font, StringFormat format, Branch branch);

		int MeasureStash(Graphics graphics, Font font, StringFormat format, StashedState stash);
	}
}
