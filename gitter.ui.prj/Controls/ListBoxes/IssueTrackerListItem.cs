namespace gitter.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	sealed class IssueTrackerListItem : CustomListBoxItem<IIssueTrackerProvider>
	{
		public IssueTrackerListItem(IIssueTrackerProvider data)
			: base(data)
		{
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintImageAndText(Data.Icon, Data.DisplayName);
					break;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureImageAndText(Data.Icon, Data.DisplayName);
				default:
					return Size.Empty;
			}
		}
	}
}
