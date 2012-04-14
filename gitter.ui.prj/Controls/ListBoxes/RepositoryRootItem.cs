namespace gitter
{
	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Controls;

	using Resources = gitter.Properties.Resources;

	public sealed class RepositoryRootItem : CustomListBoxItem
	{
		private static readonly Image ImgRepository = CachedResources.Bitmaps["ImgRepository"];

		private readonly IWorkingEnvironment _environment;
		private string _repository;

		public RepositoryRootItem(IWorkingEnvironment environment, string repository)
		{
			_environment = environment;
			_repository = repository;
			Expand();
		}

		public string RepositoryDisplayName
		{
			get { return _repository; }
			set
			{
				_repository = value;
				Invalidate();
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					if(_repository == null)
						return measureEventArgs.MeasureText("<no repository>");
					else
						return measureEventArgs.MeasureImageAndText(ImgRepository, _repository);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					if(_repository == null)
						paintEventArgs.PaintText("<no repository>");
					else
						paintEventArgs.PaintImageAndText(ImgRepository, _repository);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(_repository != null)
			{
				var menu = new ContextMenuStrip();
				var item = new ToolStripMenuItem(
					Resources.StrAddIssueTracker.AddEllipsis(), null,
					(s, e) =>
					{
						using(var d = new AddIssueTrackerDialog(_environment))
						{
							d.Run(ListBox);
						}
					});
				menu.Items.Add(item);
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			return base.GetContextMenu(requestEventArgs);
		}
	}
}
