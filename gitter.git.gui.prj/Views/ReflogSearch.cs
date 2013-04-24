namespace gitter.Git.Gui.Views
{
	using System;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	class ReflogSearch<T> : HistorySearch<T>
		where T : ReflogSearchOptions
	{
		public ReflogSearch(CustomListBox listBox)
			: base(listBox)
		{
		}

		protected static bool TestReflogRecord(ReflogRecord record, T search)
		{
			var comparison = search.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if(record.Message.IndexOf(search.Text, comparison) != -1) return true;
			return TestRevision(record.Revision, search);
		}

		protected override bool TestItem(CustomListBoxItem item, T search)
		{
			var rrli = item as ReflogRecordListItem;
			if(rrli != null)
			{
				return TestReflogRecord(rrli.DataContext, search);
			}
			return false;
		}
	}
}
