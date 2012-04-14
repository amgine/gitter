namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	/// <summary>Graph painter.</summary>
	public interface IGraphStyle
	{
		void DrawGraph(Graphics gx, GraphAtom[] graphLine, Rectangle rect, int cellWidth, RevisionGraphItemType type, bool useColors);

		void DrawReferenceConnector(Graphics gx, GraphAtom[] graphLine, int graphX, int cellWidth, int refX, int y, int h);

		void DrawReferencePresenceIndicator(Graphics gx, GraphAtom[] graphLine, int graphX, int cellWidth, int y, int h);

		
		int DrawTag(Graphics gx, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, Tag tag);

		int DrawBranch(Graphics gx, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, BranchBase branch);

		int DrawStash(Graphics gx, Font font, StringFormat format, int x, int y, int right, int h, bool hovered, StashedState stash);

		
		int MeasureTag(Graphics gx, Font font, StringFormat format, Tag tag);

		int MeasureBranch(Graphics gx, Font font, StringFormat format, Branch branch);
		
		int MeasureStash(Graphics gx, Font font, StringFormat format, StashedState stash);
	}
}
