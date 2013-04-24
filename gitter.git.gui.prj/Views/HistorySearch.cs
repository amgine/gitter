namespace gitter.Git.Gui.Views
{
	using System;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	class HistorySearch<T> : ListBoxSearch<T>
		where T : HistorySearchOptions
	{
		public HistorySearch(CustomListBox listBox)
			: base(listBox)
		{
		}

		protected static bool TestRevision(Revision revision, T search)
		{
			if(revision.Hash.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase)) return true;
			if(!revision.IsLoaded) return false;
			var comparison = search.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if(revision.Subject.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.Body.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.Author.Name.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.Committer.Name.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.TreeHash.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase)) return true;
			lock(revision.References.SyncRoot)
			{
				foreach(var reference in revision.References)
				{
					if(reference.FullName.IndexOf(search.Text, comparison) != -1) return true;
				}
			}
			return false;
		}

		protected override bool TestItem(CustomListBoxItem item, T search)
		{
			var rli = item as RevisionListItem;
			if(rli != null)
			{
				var revision = rli.DataContext;
				return TestRevision(revision, search);
			}
			return false;
		}
	}
}
