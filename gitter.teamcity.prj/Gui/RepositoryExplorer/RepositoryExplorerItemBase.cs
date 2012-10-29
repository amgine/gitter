namespace gitter.TeamCity.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	abstract class RepositoryExplorerItemBase : CustomListBoxItem
	{
		private readonly IWorkingEnvironment _environment;
		private readonly TeamCityGuiProvider _guiProvider;
		private readonly string _text;
		private Bitmap _image;

		protected RepositoryExplorerItemBase(IWorkingEnvironment env, TeamCityGuiProvider guiProvider, Bitmap image, string text)
		{
			_environment = env;
			_guiProvider = guiProvider;
			_image = image;
			_text = text;
		}

		protected IWorkingEnvironment WorkingEnvironment
		{
			get { return _environment; }
		}

		protected TeamCityGuiProvider GuiProvider
		{
			get { return _guiProvider; }
		}

		protected TeamCityServiceContext ServiceContext
		{
			get { return _guiProvider.ServiceContext; }
		}

		protected void ShowView(Guid guid)
		{
			var view = WorkingEnvironment.ViewDockService.ShowView(guid) as TeamCityViewBase;
			if(view != null)
			{
				view.ServiceContext = ServiceContext;
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
