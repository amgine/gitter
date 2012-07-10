namespace gitter.Git.Gui.Controls
{
	using System.Drawing;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	public sealed class UsresListBox : CustomListBox
	{
		#region Data

		private readonly CustomListBoxColumn _colName;
		private readonly CustomListBoxColumn _colEmail;
		private readonly CustomListBoxColumn _colCommits;

		private Repository _repository;
		private UserListBinding _binding;

		#endregion

		#region .ctor

		public UsresListBox()
		{
			Columns.AddRange(new[]
				{
					_colName		= new NameColumn(Resources.StrName),
					_colEmail		= new EmailColumn(),
					_colCommits		= new CustomListBoxColumn((int)ColumnId.Commits, Resources.StrCommits) { Width = 80 },
				});
		}

		#endregion

		public void Load(Repository repository)
		{
			if(_repository != repository)
			{
				if(_repository != null)
					DetachFromRepositoy();
				_repository = repository;
				if(_repository != null)
					AttachToRepository();
			}
		}

		private void AttachToRepository()
		{
			BeginUpdate();
			_binding = new UserListBinding(Items, _repository);
			EndUpdate();
		}

		private void DetachFromRepositoy()
		{
			BeginUpdate();
			_binding.Dispose();
			_binding = null;
			EndUpdate();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_repository != null)
				{
					_binding.Dispose();
					_binding = null;
					_repository = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
