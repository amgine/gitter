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

		protected static bool TestRevision(Revision rev, T search)
		{
			if(rev.Subject.Contains(search.Text)) return true;
			if(rev.Body.Contains(search.Text)) return true;
			if(rev.Author.Name.Contains(search.Text)) return true;
			if(rev.Committer.Name.Contains(search.Text)) return true;
			if(rev.Hash.StartsWith(search.Text)) return true;
			if(rev.TreeHash.StartsWith(search.Text)) return true;
			lock(rev.References.SyncRoot)
			{
				foreach(var reference in rev.References)
				{
					if(reference.FullName.Contains(search.Text)) return true;
				}
			}
			return false;
		}

		protected override bool TestItem(CustomListBoxItem item, T search)
		{
			var rli = item as RevisionListItem;
			if(rli != null)
			{
				return TestRevision(rli.DataContext, search);
			}
			return false;
		}
	}
}
