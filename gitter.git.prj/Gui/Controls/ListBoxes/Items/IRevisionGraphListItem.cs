namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>List item which has attached revision graph data.</summary>
	public interface IRevisionGraphListItem
	{
		/// <summary>Graph data of this item.</summary>
		GraphAtom[] Graph { get; set; }
	}
}
