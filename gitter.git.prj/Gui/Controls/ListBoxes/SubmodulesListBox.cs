namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Framework.Controls;

	public sealed class SubmodulesListBox : CustomListBox
	{
		private Repository _repository;
		private SubmoduleListBinding _binding;

		/// <summary>Create <see cref="SubmodulesListBox"/>.</summary>
		public SubmodulesListBox()
		{
			Columns.AddRange(new CustomListBoxColumn[]
				{
					new NameColumn(),
					new PathColumn() { Width = 280 },
					new UrlColumn() { Width = 280 }
				});
			Items.Comparison = SubmoduleListItem.CompareByName;
		}

		private void DetachFromRepository()
		{
			_binding.Dispose();
			_binding = null;
		}

		private void AttachToRepository()
		{
			_binding = new SubmoduleListBinding(Items, _repository);
		}

		public void Load(Repository repository)
		{
			if(_repository != repository)
			{
				if(_repository != null)
					DetachFromRepository();
				_repository = repository;
				if(_repository != null)
					AttachToRepository();
			}
		}

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
