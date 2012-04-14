namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary><see cref="CustomListBox"/> for displaying <see cref="Repository.Branches"/> &amp; <see cref="Repository.Tags"/>.</summary>
	public sealed class ReferencesListBox : CustomListBox
	{
		#region Data

		private readonly CustomListBoxColumn _colName;
		private readonly CustomListBoxColumn _colHash;
		private readonly CustomListBoxColumn _colTreeHash;

		private ReferenceTreeBinding _refBinding;

		#endregion

		public ReferencesListBox()
		{
			Columns.AddRange(new[]
				{
					_colName = new NameColumn(),
					_colHash = new HashColumn(),
					_colTreeHash = new TreeHashColumn(),
				});
			ShowTreeLines = true;
		}

		public void EnableCheckboxes()
		{
			foreach(var item in Items)
			{
				EnableCheckboxes(item);
			}
			ShowCheckBoxes = true;
		}

		private static void EnableCheckboxes(CustomListBoxItem item)
		{
			if(item is IRevisionPointerListItem)
			{
				item.CheckedState = CheckedState.Unchecked;
			}
			else
			{
				item.CheckedState = CheckedState.Unavailable;
			}
			foreach(var i in item.Items)
			{
				EnableCheckboxes(i);
			}
		}

		public void LoadData(Repository repository)
		{
			LoadData(repository, ReferenceType.Reference, true, true, null);
		}

		public void LoadData(Repository repository, ReferenceType referenceTypes, bool groupItems, bool groupRemoteBranches)
		{
			LoadData(repository, referenceTypes, groupItems, groupRemoteBranches, null);
		}

		public void LoadData(Repository repository, ReferenceType referenceTypes, bool groupItems, bool groupRemoteBranches, Predicate<IRevisionPointer> predicate)
		{
			if(_refBinding != null)
			{
				_refBinding.Dispose();
				_refBinding = null;
			}

			if(repository == null) return;

			BeginUpdate();

			_refBinding = new ReferenceTreeBinding(Items, repository, groupItems, groupRemoteBranches,
				predicate, referenceTypes);

			if(!groupRemoteBranches) ShowTreeLines = false;

			EndUpdate();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_refBinding != null)
				{
					_refBinding.Dispose();
					_refBinding = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
