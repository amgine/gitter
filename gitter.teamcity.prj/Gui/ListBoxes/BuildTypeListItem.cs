namespace gitter.TeamCity.Gui
{
	using System;
	using System.Globalization;
	using System.Drawing;

	using gitter.Framework.Controls;

	sealed class BuildTypeListItem : CustomListBoxItem<BuildType>
	{
		private static readonly Image ImgIcon = CachedResources.Bitmaps["ImgBuildType"];

		public BuildTypeListItem(BuildType buildType)
			: base(buildType)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();

			DataContext.PropertyChanged += OnBuildTypePropertyChanged;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.PropertyChanged -= OnBuildTypePropertyChanged;

			base.OnListBoxDetached();
		}

		private void OnBuildTypePropertyChanged(object sender, TeamCityObjectPropertyChangedEventArgs e)
		{
			if(e.Property == BuildType.NameProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Name);
			}
			else if(e.Property == BuildType.IdProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Id);
			}
			else if(e.Property == BuildType.WebUrlProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.WebUrl);
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Id:
					return measureEventArgs.MeasureText(DataContext.Id);
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgIcon, DataContext.Name);
				case ColumnId.WebUrl:
					return measureEventArgs.MeasureText(DataContext.WebUrl);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Id:
					paintEventArgs.PaintText(DataContext.Id);
					break;
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgIcon, DataContext.Name);
					break;
				case ColumnId.WebUrl:
					paintEventArgs.PaintText(DataContext.WebUrl);
					break;
			}
		}
	}
}
