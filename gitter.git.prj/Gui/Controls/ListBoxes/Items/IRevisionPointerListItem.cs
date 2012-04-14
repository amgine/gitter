namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Controls;

	public interface IRevisionPointerListItem
	{
		IRevisionPointer RevisionPointer { get; }
	}

	public static class RevisionPointerListItem
	{
		public static int SortComparer<T>(T item1, T item2)
			where T : CustomListBoxItem, IRevisionPointerListItem
		{
			var rev1 = item1.RevisionPointer;
			var rev2 = item2.RevisionPointer;
			return string.Compare(rev1.FullName, rev2.FullName);
		}
	}
}
