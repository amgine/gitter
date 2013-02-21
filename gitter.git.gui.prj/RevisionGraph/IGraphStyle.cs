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
