namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	public abstract class RepositoryExplorerItemBase : CustomListBoxItem
	{
		private readonly string _text;
		private Bitmap _image;
		private Repository _repository;

		protected RepositoryExplorerItemBase(Bitmap image, string text)
		{
			_image = image;
			_text = text;
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(_image, _text);
					break;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(_image, _text);
				default:
					return Size.Empty;
			}
		}

		public Repository Repository
		{
			get { return _repository; }
			set
			{
				if(_repository != value)
				{
					if(_repository != null)
					{
						DetachFromRepository();
					}
					_repository = value;
					if(_repository != null)
					{
						AttachToRepository();
					}
				}
			}
		}

		protected virtual void DetachFromRepository()
		{
		}

		protected virtual void AttachToRepository()
		{
		}
	}
}
