namespace gitter.Git.Gui.Controls
{
	/// <summary>List item which has attached revision graph data.</summary>
	public interface IRevisionGraphListItem
	{
		/// <summary>Graph data of this item.</summary>
		GraphAtom[] Graph { get; set; }
	}
}
