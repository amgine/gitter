namespace gitter.Redmine.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	public abstract class RepositoryExplorerItemBase : CustomListBoxItem
	{
		private readonly IWorkingEnvironment _environment;
		private readonly RedmineServiceContext _service;
		private readonly string _text;
		private Bitmap _image;

		protected RepositoryExplorerItemBase(IWorkingEnvironment env, RedmineServiceContext service, Bitmap image, string text)
		{
			_environment = env;
			_service = service;
			_image = image;
			_text = text;
		}

		protected IWorkingEnvironment WorkingEnvironment
		{
			get { return _environment; }
		}

		protected void ShowView(Guid guid)
		{
			var view = WorkingEnvironment.ViewDockService.ShowView(guid) as RedmineViewBase;
			if(view != null)
			{
				view.ServiceContext = _service;
			}
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
	}
}
