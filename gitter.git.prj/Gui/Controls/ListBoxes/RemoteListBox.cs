namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary><see cref="CustomListBox"/> for displaying <see cref="Repository.Remotes"/> content.</summary>
	public sealed class RemoteListBox : CustomListBox
	{
		private Repository _repository;
		private RemoteListBinding _binding;

		/// <summary>Initializes a new instance of the <see cref="RemoteListBox"/> class.</summary>
		public RemoteListBox()
		{
			Columns.AddRange(new CustomListBoxColumn[]
				{
					new NameColumn() { SizeMode = ColumnSizeMode.Sizeable, Width = 200 },
					new FetchUrlColumn() { SizeMode = ColumnSizeMode.Fill },
					new PushUrlColumn() { Width = 200 },
				});
		}

		public RemoteListBox(params CustomListBoxColumn[] columns)
		{
			if(columns == null) throw new ArgumentNullException("columns");

			Columns.AddRange(columns);
		}

		private void AttachToRepository()
		{
			BeginUpdate();
			_binding = new RemoteListBinding(Items, _repository);
			EndUpdate();
		}

		private void DetachFromRepository()
		{
			if(_binding != null)
			{
				_binding.Dispose();
				_binding = null;
			}
		}

		public void LoadData(Repository repository)
		{
			if(_repository != null)
				DetachFromRepository();
			_repository = repository;
			if(_repository != null)
				AttachToRepository();
		}

		/// <summary>Releases unmanaged and - optionally - managed resources.</summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_repository != null)
				{
					DetachFromRepository();
					_repository = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
